using CybersecurityChatbot.Models;
using CybersecurityChatbot.Services;
using System.Media;

namespace CybersecurityChatbot.Forms
{
    /// <summary>
    /// Main application form — Chat, Task Manager, Quiz (chat-driven), and Activity Log tabs.
    /// The quiz runs entirely inside the chat window: the bot sends each question as a message
    /// and replies to every answer with feedback before asking the next one.
    /// </summary>
    public partial class MainForm : Form
    {
        // ─── Services ─────────────────────────────────────────────────────────────
        private readonly ChatbotService _chatbot = new();
        private readonly QuizService _quiz = new();
        private readonly TaskService _taskService;
        private readonly DatabaseService _db = new();

        // ─── Theme Colours ────────────────────────────────────────────────────────
        private static readonly Color BgDark = Color.FromArgb(13, 17, 23);
        private static readonly Color BgCard = Color.FromArgb(22, 27, 34);
        private static readonly Color AccentGreen = Color.FromArgb(35, 197, 98);
        private static readonly Color AccentBlue = Color.FromArgb(58, 130, 246);
        private static readonly Color AccentRed = Color.FromArgb(239, 68, 68);
        private static readonly Color AccentYellow = Color.FromArgb(251, 191, 36);
        private static readonly Color TextPrimary = Color.FromArgb(230, 237, 243);
        private static readonly Color TextSecondary = Color.FromArgb(139, 148, 158);
        private static readonly Color BorderColor = Color.FromArgb(48, 54, 61);

        // ─── UI Controls — Chat ───────────────────────────────────────────────────
        private TabControl tabMain = null!;
        private RichTextBox rtbChat = null!;
        private TextBox txtInput = null!;
        private Button btnSend = null!;
        private Label lblWelcome = null!;

        // ─── UI Controls — Task ───────────────────────────────────────────────────
        private ListBox lstTasks = null!;
        private TextBox txtTaskTitle = null!;
        private TextBox txtTaskDesc = null!;
        private DateTimePicker dtpReminder = null!;
        private CheckBox chkReminder = null!;
        private Label lblTaskCount = null!;

        // ─── UI Controls — Quiz (visual tracker inside quiz tab) ─────────────────
        private Label lblQuizProgress = null!;
        private RichTextBox rtbQuizLog = null!;   // mirrors quiz messages from chat
        private Button btnStartQuizTab = null!;

        // ─── UI Controls — Log ────────────────────────────────────────────────────
        private ListBox lstLog = null!;
        private int _logDisplayCount = 10;

        // ─── State ────────────────────────────────────────────────────────────────
        private bool _nameEntered = false;
        private bool _quizActive = false;  // true while quiz is running in chat

        // ─────────────────────────────────────────────────────────────────────────
        public MainForm()
        {
            // Initialise task service — this also calls _db.InitialiseDatabase() internally
            _taskService = new TaskService(_db);

            // Attach the same DB to the chatbot so every LogActivity call is persisted
            // and past sessions are loaded back into the activity log on startup
            _chatbot.AttachDatabase(_db, _taskService.DbAvailable);

            InitialiseComponent();
            PlayGreeting();
            ShowWelcomeSequence();
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // FORM BUILD
        // ═══════════════════════════════════════════════════════════════════════════

        private void InitialiseComponent()
        {
            SuspendLayout();
            Text = "🛡️ Cybersecurity Awareness Chatbot — PROG6221";
            Size = new Size(1100, 780);
            MinimumSize = new Size(900, 650);
            BackColor = BgDark;
            ForeColor = TextPrimary;
            Font = new Font("Segoe UI", 9.5f);
            StartPosition = FormStartPosition.CenterScreen;

            tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = BgDark,
                Padding = new Point(12, 6)
            };

            BuildChatTab();
            BuildTaskTab();
            BuildQuizTab();
            BuildLogTab();

            Controls.Add(tabMain);
            ResumeLayout(false);
        }

        // ── Chat Tab ──────────────────────────────────────────────────────────────
        private void BuildChatTab()
        {
            var tab = new TabPage("💬  Chat") { BackColor = BgDark };

            // ASCII header banner
            var lblAscii = new Label
            {
                Text = "  🛡️  CYBERSECURITY AWARENESS ASSISTANT  🛡️\n        Protecting South African Citizens",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = AccentGreen,
                BackColor = BgCard,
                Dock = DockStyle.Top,
                Height = 55,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblWelcome = new Label
            {
                Text = "Welcome! What is your name?",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = AccentBlue,
                BackColor = BgDark,
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter
            };

            rtbChat = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Padding = new Padding(8)
            };

            // Input row
            var pnlInput = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = BgDark, Padding = new Padding(0, 5, 0, 0) };

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Type your message here..."
            };
            txtInput.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SendMessage(); } };

            btnSend = new Button
            {
                Text = "Send ➤",
                Dock = DockStyle.Right,
                Width = 110,
                BackColor = AccentGreen,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += (s, e) => SendMessage();

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);

            tab.Controls.Add(rtbChat);
            tab.Controls.Add(lblWelcome);
            tab.Controls.Add(lblAscii);
            tab.Controls.Add(pnlInput);
            tabMain.TabPages.Add(tab);
        }

        // ── Task Tab ──────────────────────────────────────────────────────────────
        private void BuildTaskTab()
        {
            var tab = new TabPage("✅  Task Manager") { BackColor = BgDark };
            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = BgDark, Padding = new Padding(12) };

            var lblHeader = CreateSectionLabel("📋 Cybersecurity Task Assistant");

            lstTasks = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTaskCount = new Label { Dock = DockStyle.Top, Height = 24, ForeColor = TextSecondary, BackColor = BgDark, TextAlign = ContentAlignment.MiddleLeft, Text = "0 tasks" };

            // Right-side input panel
            var pnlRight = new Panel { Dock = DockStyle.Right, Width = 320, BackColor = BgCard, Padding = new Padding(10), BorderStyle = BorderStyle.FixedSingle };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, BackColor = BgCard, Padding = new Padding(5) };

            void Gap(int h = 8) => flow.Controls.Add(new Panel { Width = 290, Height = h, BackColor = BgCard });

            txtTaskTitle = new TextBox { Width = 290, BackColor = BgDark, ForeColor = TextPrimary, Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "e.g. Enable 2FA on Gmail" };
            txtTaskDesc = new TextBox { Width = 290, Height = 60, Multiline = true, BackColor = BgDark, ForeColor = TextPrimary, Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Optional description..." };
            chkReminder = new CheckBox { Text = "Set Reminder Date", ForeColor = TextPrimary, AutoSize = true };
            dtpReminder = new DateTimePicker { Width = 290, Format = DateTimePickerFormat.Short, BackColor = BgDark, ForeColor = TextPrimary, Enabled = false, MinDate = DateTime.Today };
            chkReminder.CheckedChanged += (s, e) => dtpReminder.Enabled = chkReminder.Checked;

            var btnAdd = CreateStyledButton("➕ Add Task", AccentGreen, Color.Black); btnAdd.Width = 290; btnAdd.Click += BtnAddTask_Click;
            var btnDone = CreateStyledButton("✅ Mark Complete", AccentBlue, Color.White); btnDone.Width = 290; btnDone.Click += BtnCompleteTask_Click;
            var btnDel = CreateStyledButton("🗑️ Delete Task", AccentRed, Color.White); btnDel.Width = 290; btnDel.Click += BtnDeleteTask_Click;

            flow.Controls.Add(CreateSectionLabelInline("➕ Add New Task")); Gap();
            flow.Controls.Add(new Label { Text = "Task Title:", ForeColor = TextSecondary, AutoSize = true });
            flow.Controls.Add(txtTaskTitle); Gap();
            flow.Controls.Add(new Label { Text = "Description:", ForeColor = TextSecondary, AutoSize = true });
            flow.Controls.Add(txtTaskDesc); Gap();
            flow.Controls.Add(chkReminder); flow.Controls.Add(dtpReminder); Gap(12);
            flow.Controls.Add(btnAdd); Gap(20);
            flow.Controls.Add(new Label { Text = "── Selected Task Actions ──", ForeColor = TextSecondary, AutoSize = true }); Gap(4);
            flow.Controls.Add(btnDone); Gap(); flow.Controls.Add(btnDel);

            pnlRight.Controls.Add(flow);

            var pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = BgDark, Padding = new Padding(0, 0, 8, 0) };
            pnlLeft.Controls.Add(lstTasks);
            pnlLeft.Controls.Add(lblTaskCount);
            pnlLeft.Controls.Add(lblHeader);

            pnl.Controls.Add(pnlLeft);
            pnl.Controls.Add(pnlRight);
            tab.Controls.Add(pnl);
            tabMain.TabPages.Add(tab);
            tabMain.SelectedIndexChanged += (s, e) => { if (tabMain.SelectedIndex == 1) RefreshTaskList(); };
        }

        // ── Quiz Tab — fully self-contained with clickable options & instant bot replies ──
        // rtbQuizLog is now the quiz's own conversation window.
        // pnlQuizOptions holds the clickable answer buttons.
        // After every answer the bot replies with feedback before the user moves on.

        private Panel pnlQuizOptions = null!;
        private Button btnNextQuestion = null!;

        private void BuildQuizTab()
        {
            var tab = new TabPage("🎮  Quiz") { BackColor = BgDark };

            // Header
            var lblHeader = CreateSectionLabel("🎮 Cybersecurity Quiz — Click an answer, the bot replies instantly!");

            // Score / progress bar
            lblQuizProgress = new Label
            {
                Text = "Press ▶ Start Quiz to begin.",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = AccentYellow,
                BackColor = BgCard,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Quiz conversation window (acts like a chat window)
            rtbQuizLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Padding = new Padding(6),
                Text = "Welcome to the Quiz! Press ▶ Start Quiz below to begin.\n"
            };

            // Clickable answer buttons panel
            pnlQuizOptions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 190,
                BackColor = BgDark,
                Padding = new Padding(4)
            };

            // Bottom button bar
            var pnlBar = new Panel { Dock = DockStyle.Bottom, Height = 46, BackColor = BgDark };

            btnStartQuizTab = CreateStyledButton("▶  Start Quiz", AccentGreen, Color.Black);
            btnStartQuizTab.Width = 150;
            btnStartQuizTab.Height = 38;
            btnStartQuizTab.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnStartQuizTab.Click += (s, e) => StartQuizOnTab();

            btnNextQuestion = CreateStyledButton("Next Question ▶", AccentBlue, Color.White);
            btnNextQuestion.Width = 160;
            btnNextQuestion.Height = 38;
            btnNextQuestion.Left = 160;
            btnNextQuestion.Enabled = false;
            btnNextQuestion.Click += (s, e) => ShowNextQuizQuestion();

            pnlBar.Controls.Add(btnStartQuizTab);
            pnlBar.Controls.Add(btnNextQuestion);

            tab.Controls.Add(rtbQuizLog);
            tab.Controls.Add(pnlQuizOptions);
            tab.Controls.Add(pnlBar);
            tab.Controls.Add(lblQuizProgress);
            tab.Controls.Add(lblHeader);
            tabMain.TabPages.Add(tab);
        }

        // ── Start quiz on the Quiz tab ────────────────────────────────────────────
        private void StartQuizOnTab()
        {
            _quiz.StartQuiz();
            _chatbot.LogActivity("Quiz", "Quiz started (Quiz tab)");
            rtbQuizLog.Clear();
            pnlQuizOptions.Controls.Clear();
            btnNextQuestion.Enabled = false;
            UpdateQuizProgressLabel();

            QuizChatAppend("BOT",
                "🎮 Welcome to the Cybersecurity Quiz!\n" +
                $"There are {_quiz.TotalQuestions} questions.\n" +
                "Click one of the option buttons below to answer. " +
                "I'll reply with feedback straight away before we move on!",
                AccentYellow);

            QuizChatAppend("BOT", "──────────────────────────────────", TextSecondary);
            RenderCurrentQuizQuestion();
        }

        // ── Render the current question with clickable option buttons ─────────────
        private void RenderCurrentQuizQuestion()
        {
            var q = _quiz.GetCurrentQuestion();
            if (q == null) return;

            // Bot posts the question
            string msg = $"❓ Question {_quiz.CurrentIndex + 1} of {_quiz.TotalQuestions}\n\n{q.Question}";
            QuizChatAppend("BOT", msg, AccentBlue);

            // Build option buttons
            pnlQuizOptions.Controls.Clear();
            btnNextQuestion.Enabled = false;

            int x = 0;
            for (int i = 0; i < q.Options.Count; i++)
            {
                int idx = i;
                string letter = q.IsTrueFalse
                    ? (i == 0 ? "True" : "False")
                    : ((char)('A' + i)).ToString();

                var btn = new Button
                {
                    Text = $"  {letter})  {q.Options[i]}",
                    Location = new Point(x, 8),
                    Width = 240,
                    Height = 168,
                    BackColor = BgCard,
                    ForeColor = TextPrimary,
                    Font = new Font("Segoe UI", 10f),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderColor = BorderColor;
                btn.Click += (s, e) => OnQuizOptionClicked(idx);
                pnlQuizOptions.Controls.Add(btn);
                x += 248;
            }
        }

        // ── User clicks an answer button ──────────────────────────────────────────
        private void OnQuizOptionClicked(int selectedIndex)
        {
            // Disable all options so user can't double-click
            foreach (Control c in pnlQuizOptions.Controls)
                if (c is Button b) { b.Enabled = false; b.BackColor = BgDark; b.ForeColor = TextSecondary; }

            var q = _quiz.GetCurrentQuestion()!;
            string letter = q.IsTrueFalse
                ? (selectedIndex == 0 ? "True" : "False")
                : ((char)('A' + selectedIndex)).ToString();

            // Echo user's choice in the conversation
            QuizChatAppend(_chatbot.UserName, $"My answer: {letter}) {q.Options[selectedIndex]}", Color.FromArgb(200, 220, 255));

            // Submit and get feedback
            var (isCorrect, feedback) = _quiz.SubmitAnswer(selectedIndex);
            _chatbot.LogActivity("Quiz",
                $"Q{_quiz.CurrentIndex} answered '{letter}' — {(isCorrect ? "Correct ✅" : "Incorrect ❌")}");

            // Colour the chosen button and highlight the correct one
            if (pnlQuizOptions.Controls.Count > q.CorrectOptionIndex
                && pnlQuizOptions.Controls[q.CorrectOptionIndex] is Button correctBtn)
            {
                correctBtn.BackColor = AccentGreen;
                correctBtn.ForeColor = Color.Black;
            }
            if (!isCorrect && pnlQuizOptions.Controls.Count > selectedIndex
                && pnlQuizOptions.Controls[selectedIndex] is Button wrongBtn)
            {
                wrongBtn.BackColor = AccentRed;
                wrongBtn.ForeColor = Color.White;
            }

            // Bot replies with feedback
            Color feedbackColour = isCorrect ? AccentGreen : AccentRed;
            QuizChatAppend("BOT", feedback, feedbackColour);
            QuizChatAppend("BOT", "──────────────────────────────────", TextSecondary);

            UpdateQuizProgressLabel();

            // Quiz over?
            if (_quiz.CurrentIndex >= _quiz.TotalQuestions)
            {
                string finalMsg = _quiz.GetFinalMessage();
                QuizChatAppend("BOT",
                    $"🏁 Quiz Complete!\n\n{finalMsg}\n\nPress ▶ Start Quiz to play again!",
                    AccentYellow);
                _chatbot.LogActivity("Quiz", $"Quiz complete — Score: {_quiz.Score}/{_quiz.TotalQuestions}");
                pnlQuizOptions.Controls.Clear();
                btnNextQuestion.Enabled = false;
            }
            else
            {
                // Let user read the feedback, then click Next
                btnNextQuestion.Enabled = true;
            }
        }

        // ── Advance to next question ──────────────────────────────────────────────
        private void ShowNextQuizQuestion()
        {
            btnNextQuestion.Enabled = false;
            RenderCurrentQuizQuestion();
        }

        // ── Append to the quiz conversation window ────────────────────────────────
        private void QuizChatAppend(string speaker, string message, Color colour)
        {
            rtbQuizLog.SelectionStart = rtbQuizLog.TextLength;

            rtbQuizLog.SelectionColor = TextSecondary;
            rtbQuizLog.AppendText($"\n[{DateTime.Now:HH:mm:ss}] ");

            rtbQuizLog.SelectionColor = colour;
            rtbQuizLog.SelectionFont = new Font("Consolas", 10f, FontStyle.Bold);
            rtbQuizLog.AppendText($"{speaker}:\n");

            rtbQuizLog.SelectionFont = new Font("Consolas", 10f);
            rtbQuizLog.SelectionColor = speaker == "BOT" ? TextPrimary : Color.FromArgb(200, 220, 255);
            rtbQuizLog.AppendText($"{message}\n");

            rtbQuizLog.ScrollToCaret();
        }

        // ── Log Tab ───────────────────────────────────────────────────────────────
        private void BuildLogTab()
        {
            var tab = new TabPage("📋  Activity Log") { BackColor = BgDark };
            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = BgDark, Padding = new Padding(12) };

            var lblHeader = CreateSectionLabel("📋 Activity Log — Recent Chatbot Actions");

            lstLog = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = AccentGreen,
                Font = new Font("Consolas", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.None
            };

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 45, BackColor = BgDark };
            var btnRefresh = CreateStyledButton("🔄 Refresh", AccentBlue, Color.White); btnRefresh.Width = 140; btnRefresh.Click += (s, e) => RefreshLog();
            var btnMore = CreateStyledButton("📂 Show More", AccentGreen, Color.Black); btnMore.Width = 140; btnMore.Left = 150;
            btnMore.Click += (s, e) => { _logDisplayCount += 10; RefreshLog(); };
            pnlBtns.Controls.Add(btnRefresh);
            pnlBtns.Controls.Add(btnMore);

            pnl.Controls.Add(lstLog);
            pnl.Controls.Add(pnlBtns);
            pnl.Controls.Add(lblHeader);
            tab.Controls.Add(pnl);
            tabMain.TabPages.Add(tab);
            tabMain.SelectedIndexChanged += (s, e) => { if (tabMain.SelectedIndex == 3) RefreshLog(); };
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // VOICE GREETING & WELCOME
        // ═══════════════════════════════════════════════════════════════════════════

        private void PlayGreeting()
        {
            try
            {
                string wav = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "greeting.wav");
                if (File.Exists(wav)) new SoundPlayer(wav).Play();
            }
            catch { }
        }

        private void ShowWelcomeSequence()
        {
            AppendChat("BOT", ChatbotService.GetAsciiArt(), AccentGreen);
            AppendChat("BOT", "Hello! Welcome to the Cybersecurity Awareness Bot. 🛡️\nI'm here to help South African citizens stay safe online.", AccentBlue);

            // If the DB is available and there are previous log entries, greet as returning user
            bool hasHistory = _taskService.DbAvailable && _chatbot.ActivityLog.Count > 0;
            if (hasHistory)
            {
                int taskCount = _taskService.Tasks.Count;
                int pending = _taskService.Tasks.Count(t => !t.IsCompleted);
                int logCount = _chatbot.ActivityLog.Count;

                string returnMsg =
                    $"👋 Welcome back! Here's a quick summary of where you left off:\n\n" +
                    $"  📋 Tasks: {taskCount} total, {pending} still pending\n" +
                    $"  📓 Activity log: {logCount} entries restored from your last session\n\n" +
                    "Your tasks are loaded in the Task Manager tab. " +
                    "Type 'show log' to see your recent activity.";

                AppendChat("BOT", returnMsg, AccentYellow);
            }

            AppendChat("BOT", "Before we begin — what is your name?", AccentGreen);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // SEND MESSAGE — MAIN ROUTING HUB
        // ═══════════════════════════════════════════════════════════════════════════

        private void SendMessage()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;
            txtInput.Clear();

            // Always echo the user's message in chat first
            AppendChat(_nameEntered ? _chatbot.UserName : "You", input, Color.FromArgb(200, 220, 255));

            // ── Step 1: Name entry on first message ───────────────────────────────
            if (!_nameEntered)
            {
                _chatbot.UserName = input;
                _nameEntered = true;
                lblWelcome.Text = $"Welcome, {_chatbot.UserName}! Chat · Tasks · Quiz · Log";
                lblWelcome.ForeColor = AccentGreen;
                _chatbot.LogActivity("Chat", $"User registered as: {_chatbot.UserName}");
                AppendChat("BOT",
                    $"Great to meet you, {_chatbot.UserName}! 😊\n\n" +
                    "I'm your Cybersecurity Awareness Assistant. You can:\n" +
                    "  💬 Ask me about passwords, phishing, scams, privacy, malware, 2FA, browsing\n" +
                    "  🎮 Type 'start quiz' to test your cybersecurity knowledge\n" +
                    "  ✅ Type 'add task' to create a cybersecurity to-do\n" +
                    "  📋 Type 'show log' to see recent actions\n\n" +
                    "What would you like to know?",
                    AccentGreen);
                return;
            }

            // ── Step 2: If quiz is active, route to quiz answer handler ──────────
            if (_quizActive)
            {
                HandleQuizAnswer(input);
                return;
            }

            // ── Step 3: NLP routing for special intents ───────────────────────────
            string lower = input.ToLower();

            if (ContainsAny(lower, "start quiz", "play quiz", "quiz me", "begin quiz", "take quiz"))
            {
                StartQuizInChat();
                return;
            }

            if (ContainsAny(lower, "add task", "new task", "create task", "remind me to", "set reminder"))
            {
                HandleNlpTaskAdd(input);
                return;
            }

            if (ContainsAny(lower, "show tasks", "list tasks", "my tasks", "view tasks"))
            {
                _chatbot.LogActivity("Task", "User requested task list via chat");
                AppendChat("BOT", _taskService.GetTaskListString(), AccentBlue);
                return;
            }

            if (ContainsAny(lower, "show log", "activity log", "what have you done", "recent actions", "show activity"))
            {
                AppendChat("BOT", _chatbot.GetActivityLogSummary(), AccentBlue);
                return;
            }

            // ── Step 4: Normal chatbot response ───────────────────────────────────
            string response = _chatbot.GetResponse(input);
            AppendChat("BOT", response, AccentGreen);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // QUIZ — RUNS ENTIRELY IN THE CHAT WINDOW
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Starts the quiz in the chat window. The bot sends the first question as a message.
        /// </summary>
        private void StartQuizInChat()
        {
            _quiz.StartQuiz();
            _quizActive = true;
            _chatbot.LogActivity("Quiz", "Quiz started");
            UpdateQuizProgressLabel();

            AppendChat("BOT",
                "🎮 Starting the Cybersecurity Quiz!\n\n" +
                $"There are {_quiz.TotalQuestions} questions. Type the letter (A / B / C / D) " +
                "or True / False to answer each one.\n\n" +
                "Type 'quit quiz' at any time to stop.\n\n" +
                "──────────────────────────────────",
                AccentYellow);

            AskCurrentQuestion();
        }

        /// <summary>
        /// Posts the current quiz question into the chat as a bot message.
        /// </summary>
        private void AskCurrentQuestion()
        {
            var q = _quiz.GetCurrentQuestion();
            if (q == null) return;

            // Build the question message
            string msg = $"❓ Question {_quiz.CurrentIndex + 1} of {_quiz.TotalQuestions}\n\n";
            msg += q.Question + "\n\n";

            if (q.IsTrueFalse)
            {
                msg += "  A)  True\n";
                msg += "  B)  False\n";
            }
            else
            {
                string[] letters = { "A", "B", "C", "D" };
                for (int i = 0; i < q.Options.Count; i++)
                    msg += $"  {letters[i]})  {q.Options[i]}\n";
            }

            msg += "\nType your answer (A / B / C / D):";
            AppendChat("BOT", msg, AccentBlue);

            // Mirror to quiz tab conversation window
            QuizChatAppend("BOT", msg, AccentBlue);
        }

        /// <summary>
        /// Processes the user's typed answer while the quiz is active.
        /// </summary>
        private void HandleQuizAnswer(string input)
        {
            string trimmed = input.Trim().ToUpper();

            // Allow quitting
            if (trimmed == "QUIT QUIZ" || trimmed == "STOP QUIZ" || trimmed == "EXIT QUIZ")
            {
                _quizActive = false;
                AppendChat("BOT", $"Quiz stopped. You had {_quiz.Score} correct so far. Feel free to start again anytime!", AccentYellow);
                _chatbot.LogActivity("Quiz", $"Quiz quit early — Score so far: {_quiz.Score}");
                UpdateQuizProgressLabel();
                return;
            }

            // Parse answer letter → index
            int selectedIndex = trimmed switch
            {
                "A" or "TRUE" => 0,
                "B" or "FALSE" => 1,
                "C" => 2,
                "D" => 3,
                _ => -1
            };

            var currentQ = _quiz.GetCurrentQuestion();

            if (selectedIndex < 0 || (currentQ != null && selectedIndex >= currentQ.Options.Count))
            {
                AppendChat("BOT",
                    "⚠️ Please type a valid answer: A, B, C, or D (or True / False for true/false questions).",
                    AccentRed);
                return;
            }

            // Submit and get feedback
            var (isCorrect, feedback) = _quiz.SubmitAnswer(selectedIndex);
            _chatbot.LogActivity("Quiz",
                $"Q{_quiz.CurrentIndex} — User answered '{trimmed}' — {(isCorrect ? "Correct ✅" : "Incorrect ❌")}");

            // Bot replies with feedback immediately in both windows
            Color feedbackColor = isCorrect ? AccentGreen : AccentRed;
            AppendChat("BOT", feedback, feedbackColor);
            QuizChatAppend("BOT", feedback, feedbackColor);

            UpdateQuizProgressLabel();

            // Quiz finished?
            if (_quiz.CurrentIndex >= _quiz.TotalQuestions)
            {
                _quizActive = false;
                string finalMsg = _quiz.GetFinalMessage();
                string endMsg = $"\n🏁 Quiz Complete!\n\n{finalMsg}\n\nType 'start quiz' anytime to play again!";
                AppendChat("BOT", endMsg, AccentYellow);
                QuizChatAppend("BOT", endMsg, AccentYellow);
                _chatbot.LogActivity("Quiz", $"Quiz complete — Final score: {_quiz.Score}/{_quiz.TotalQuestions}");
                UpdateQuizProgressLabel();
            }
            else
            {
                // Separator then next question in both windows
                AppendChat("BOT", "──────────────────────────────────", TextSecondary);
                QuizChatAppend("BOT", "──────────────────────────────────", TextSecondary);
                AskCurrentQuestion();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // NLP TASK CREATION
        // ═══════════════════════════════════════════════════════════════════════════

        private void HandleNlpTaskAdd(string input)
        {
            string title = input;
            string[] prefixes = {
                "add a task to", "add task to", "add a task", "add task",
                "new task", "create task", "remind me to", "set a reminder to",
                "set reminder for", "set reminder to"
            };

            foreach (var prefix in prefixes)
            {
                if (title.ToLower().StartsWith(prefix))
                {
                    title = title.Substring(prefix.Length).Trim();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(title)) title = "New cybersecurity task";
            title = char.ToUpper(title[0]) + title.Substring(1);

            DateTime? reminder = null;
            string lower = input.ToLower();
            if (lower.Contains("tomorrow")) reminder = DateTime.Today.AddDays(1);
            else if (lower.Contains("in 3 day")) reminder = DateTime.Today.AddDays(3);
            else if (lower.Contains("in 5 day")) reminder = DateTime.Today.AddDays(5);
            else if (lower.Contains("in 7 day") || lower.Contains("in a week"))
                reminder = DateTime.Today.AddDays(7);

            var task = _taskService.AddTask(title, "Added via chat.", reminder);
            _chatbot.LogActivity("Task",
                $"Task added: '{title}'" + (reminder.HasValue ? $" (Reminder: {reminder.Value:dd MMM yyyy})" : ""));

            string msg = $"✅ Task added: '{title}'\n";
            msg += reminder.HasValue
                ? $"🔔 Reminder set for {reminder.Value:dd MMM yyyy}."
                : "No reminder set. You can say e.g. 'remind me in 3 days' to add one.";

            AppendChat("BOT", msg, AccentGreen);
            RefreshTaskList();
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // TASK TAB — EVENT HANDLERS
        // ═══════════════════════════════════════════════════════════════════════════

        private void BtnAddTask_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTaskTitle.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime? reminder = chkReminder.Checked ? dtpReminder.Value.Date : (DateTime?)null;
            var task = _taskService.AddTask(txtTaskTitle.Text.Trim(), txtTaskDesc.Text.Trim(), reminder);
            _chatbot.LogActivity("Task",
                $"Task added: '{task.Title}'" + (reminder.HasValue ? $" (Reminder: {reminder.Value:dd MMM yyyy})" : ""));
            txtTaskTitle.Clear(); txtTaskDesc.Clear(); chkReminder.Checked = false;
            RefreshTaskList();
        }

        private void BtnCompleteTask_Click(object? sender, EventArgs e)
        {
            if (lstTasks.SelectedItem is ChatTask task)
            {
                if (task.IsCompleted) { MessageBox.Show("Already completed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                _taskService.MarkComplete(task.Id);
                _chatbot.LogActivity("Task", $"Task completed: '{task.Title}'");
                RefreshTaskList();
            }
        }

        private void BtnDeleteTask_Click(object? sender, EventArgs e)
        {
            if (lstTasks.SelectedItem is ChatTask task)
            {
                if (MessageBox.Show($"Delete '{task.Title}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _taskService.DeleteTask(task.Id);
                    _chatbot.LogActivity("Task", $"Task deleted: '{task.Title}'");
                    RefreshTaskList();
                }
            }
        }

        private void RefreshTaskList()
        {
            lstTasks.Items.Clear();
            foreach (var t in _taskService.Tasks) lstTasks.Items.Add(t);
            lblTaskCount.Text = $"{_taskService.Tasks.Count} task(s) | {_taskService.Tasks.Count(t => t.IsCompleted)} completed";
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ACTIVITY LOG
        // ═══════════════════════════════════════════════════════════════════════════

        private void RefreshLog()
        {
            lstLog.Items.Clear();
            var entries = _chatbot.ActivityLog.Take(_logDisplayCount).ToList();
            foreach (var e in entries) lstLog.Items.Add(e.ToString());
            if (_chatbot.ActivityLog.Count == 0) lstLog.Items.Add("No activity yet.");
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // QUIZ TAB HELPERS
        // ═══════════════════════════════════════════════════════════════════════════

        private void UpdateQuizProgressLabel()
        {
            if (_quizActive)
                lblQuizProgress.Text = $"In progress — Q{_quiz.CurrentIndex + 1}/{_quiz.TotalQuestions} | Score: {_quiz.Score}";
            else if (_quiz.CurrentIndex >= _quiz.TotalQuestions && _quiz.TotalQuestions > 0)
                lblQuizProgress.Text = $"Last result: {_quiz.Score}/{_quiz.TotalQuestions} correct";
            else
                lblQuizProgress.Text = "No quiz in progress.";
        }

        // AppendQuizLog replaced by QuizChatAppend above

        // ═══════════════════════════════════════════════════════════════════════════
        // CHAT DISPLAY
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Appends a timestamped, colour-coded message to the chat box.
        /// </summary>
        private void AppendChat(string speaker, string message, Color colour)
        {
            if (InvokeRequired) { Invoke(() => AppendChat(speaker, message, colour)); return; }

            rtbChat.SelectionStart = rtbChat.TextLength;

            // Timestamp
            rtbChat.SelectionColor = TextSecondary;
            rtbChat.AppendText($"\n[{DateTime.Now:HH:mm:ss}] ");

            // Speaker label
            rtbChat.SelectionColor = colour;
            rtbChat.SelectionFont = new Font("Consolas", 10f, FontStyle.Bold);
            rtbChat.AppendText($"{speaker}:\n");

            // Message body
            rtbChat.SelectionColor = speaker == "BOT" ? TextPrimary : Color.FromArgb(200, 220, 255);
            rtbChat.SelectionFont = new Font("Consolas", 10f);
            rtbChat.AppendText($"{message}\n");

            // Divider
            rtbChat.SelectionColor = BorderColor;
            rtbChat.AppendText("─────────────────────────────────────────\n");

            rtbChat.ScrollToCaret();
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // UI FACTORY HELPERS
        // ═══════════════════════════════════════════════════════════════════════════

        private static Label CreateSectionLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 197, 98),
            BackColor = Color.FromArgb(22, 27, 34),
            Dock = DockStyle.Top,
            Height = 36,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(4, 0, 0, 0)
        };

        private static Label CreateSectionLabelInline(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 197, 98),
            BackColor = Color.FromArgb(22, 27, 34),
            AutoSize = false,
            Width = 290,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft
        };

        private static Button CreateStyledButton(string text, Color back, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Height = 36,
                BackColor = back,
                ForeColor = fore,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static bool ContainsAny(string input, params string[] kws)
            => kws.Any(k => input.Contains(k));
    }
}
