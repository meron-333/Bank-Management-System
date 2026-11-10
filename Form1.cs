
using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace BankManagementSystem
{
    public partial class Form1 : Form
    {
        //============================
        // create db conection
        //============================
        private readonly string connectionString =
            "Host=localhost;Port=54321;Database=bank_management;Username=postgres;Password=admin";

        //=================================
        // check db connection
        //=================================

        private void TestDatabaseConnection()
        {
            try
            {
                using var connection =
                    new Npgsql.NpgsqlConnection(connectionString);

                connection.Open();

                MessageBox.Show("PostgreSQL connected successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message);
            }
        }
        private Panel sidebar, mainPanel, headerPanel;
        private Panel contentPanel, editorPanel, gridPanel;
        private DataGridView clientsGrid;

        private TextBox txtName, txtPhone, txtEmail, txtGender, txtDate_of_birth, txtBalance, txtAccount_number, txtAccount_type;
        private TextBox txtAddress, txtSearch;

        private Label lblTotalClients, lblStatus;

        private readonly Color dark =
            Color.FromArgb(35, 43, 55);

        private readonly Color accent =
            Color.FromArgb(45, 110, 190);

        private readonly Color background =
            Color.FromArgb(243, 246, 250);

        public Form1()
        {
            InitializeComponent();

            BuildDashboard();
            LoadClients();
        }

        // ===============================
        // 1. BUILD DASHBOARD
        // ===============================

        private void BuildDashboard()
        {
            Text = "Bank Management System";
            Size = new Size(1250, 780);
            MinimumSize = new Size(1050, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = background;

            // Sidebar
            sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 220;
            sidebar.BackColor = dark;

            Label logo = new Label();
            logo.Text = "BANK SYSTEM";
            logo.Dock = DockStyle.Top;
            logo.Height = 110;
            logo.ForeColor = Color.White;
            logo.Font = new Font("Segoe UI", 19, FontStyle.Bold);
            logo.TextAlign = ContentAlignment.MiddleCenter;

            sidebar.Controls.Add(logo);

            AddMenuButton("Dashboard", 120);
            AddMenuButton("Clients", 180);
            AddMenuButton("Users", 240);
            AddMenuButton("Transactions", 300);
            AddMenuButton("Login Register", 360);
            AddMenuButton("Logout", 450);

            // Main area
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = background;

            // Header
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 75;
            headerPanel.BackColor = Color.White;

            Label admin = new Label();
            admin.Text = "Customer Management ";
            admin.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            admin.Location = new Point(25, 18);
            admin.AutoSize = true;

            headerPanel.Controls.Add(admin);

            // Main content
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = background;
            contentPanel.Padding = new Padding(20);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(headerPanel);

            Controls.Add(mainPanel);
            Controls.Add(sidebar);

            BuildContent();
        }

        // ===============================
        // 2. SIDEBAR BUTTONS
        // ===============================

        private void AddMenuButton(string text, int top)
        {
            Button button = new Button();

            button.Text = text;
            button.SetBounds(0, top, 220, 55);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.BackColor = dark;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            button.Click += (s, e) =>
            {
                if (text == "Clients" || text == "Dashboard")
                {
                    BuildContent();
                    LoadClients();
                }
                else if (text == "Logout")
                {
                    Logout();
                }
                else
                {
                    MessageBox.Show(
                        text + " module will be developed next.");
                }
            };

            sidebar.Controls.Add(button);
        }

        // ===============================
        // 3. BUILD CLIENT CONTENT
        // ===============================

        private void BuildContent()
        {
            contentPanel.Controls.Clear();

            Label title = new Label();

            title.Font = new Font("Segoe UI", 21, FontStyle.Bold);
            title.Location = new Point(20, 10);
            title.AutoSize = true;

            Label description = new Label();
            description.Text = "Manage and organize bank clients.";

            description.Font = new Font("Segoe UI", 10);
            description.ForeColor = Color.Gray;
            description.Location = new Point(23, 50);
            description.AutoSize = true;

            // Statistics card
            Panel statsCard = new Panel();
            statsCard.SetBounds(20, 85, 260, 85);
            statsCard.BackColor = Color.White;

            Label statsTitle = new Label();
            statsTitle.Text = "TOTAL CLIENTS";
            statsTitle.Location = new Point(15, 12);
            statsTitle.AutoSize = true;
            statsTitle.ForeColor = Color.Gray;

            lblTotalClients = new Label();
            lblTotalClients.Text = "0";
            lblTotalClients.Location = new Point(15, 38);
            lblTotalClients.AutoSize = true;
            lblTotalClients.Font =
                new Font("Segoe UI", 21, FontStyle.Bold);
            lblTotalClients.ForeColor = accent;

            statsCard.Controls.Add(statsTitle);
            statsCard.Controls.Add(lblTotalClients);

            // Editor panel
            editorPanel = new Panel();
            editorPanel.SetBounds(20, 190, 310, 420);
            editorPanel.BackColor = Color.White;
            editorPanel.Padding = new Padding(15);

            BuildEditor();

            // Grid panel
            gridPanel = new Panel();
            gridPanel.SetBounds(350, 190, 800, 420);
            gridPanel.BackColor = Color.White;

            BuildGrid();

            lblStatus = new Label();
            lblStatus.Text = "Ready";
            lblStatus.Location = new Point(20, 620);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Gray;

            contentPanel.Controls.Add(title);
            contentPanel.Controls.Add(description);
            contentPanel.Controls.Add(statsCard);
            contentPanel.Controls.Add(editorPanel);
            contentPanel.Controls.Add(gridPanel);
            contentPanel.Controls.Add(lblStatus);
        }

        // ===============================
        // 4. CLIENT INPUT FORM
        // ===============================

        private void BuildEditor()
        {
            Label heading = new Label();
            heading.Text = "Client Information";
            heading.Font =
                new Font("Segoe UI", 13, FontStyle.Bold);
            heading.SetBounds(15, 12, 260, 30);

            editorPanel.Controls.Add(heading);

            txtName = CreateTextBox("Full Name", 55);
            txtPhone = CreateTextBox("Phone Number", 115);
            txtEmail = CreateTextBox("Email Address", 175);
            txtGender = CreateTextBox("gender", 235);

            txtAddress = CreateTextBox("adress", 115);


            AddEditorButton("Add Client", 295, accent, AddClient);
            AddEditorButton("Update Client", 340, dark, UpdateClient);
            AddEditorButton("Delete Client", 385,
                Color.FromArgb(190, 55, 55), DeleteClient);
        }

        private TextBox CreateTextBox(string labelText, int y)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(15, y);
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9);

            TextBox box = new TextBox();
            box.SetBounds(15, y + 20, 260, 27);
            box.Font = new Font("Segoe UI", 10);

            editorPanel.Controls.Add(label);
            editorPanel.Controls.Add(box);

            return box;
        }

        private void AddEditorButton(
            string text, int y, Color color, Action action)
        {
            Button button = new Button();

            button.Text = text;
            button.SetBounds(15, y, 260, 38);
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            button.Click += (s, e) => action();

            editorPanel.Controls.Add(button);
        }

        // ===============================
        // 5. CLIENT DATA GRID
        // ===============================

        private void BuildGrid()
        {
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 55;

            txtSearch = new TextBox();
            txtSearch.SetBounds(12, 12, 250, 28);
            txtSearch.PlaceholderText = "Search client name or ID";

            Button searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.SetBounds(275, 10, 90, 32);
            searchButton.BackColor = accent;
            searchButton.ForeColor = Color.White;
            searchButton.FlatStyle = FlatStyle.Flat;

            searchButton.Click += (s, e) =>
                LoadClients(txtSearch.Text.Trim());

            Button refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.SetBounds(375, 10, 90, 32);

            refreshButton.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadClients();
            };

            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(searchButton);
            searchPanel.Controls.Add(refreshButton);

            clientsGrid = new DataGridView();
            clientsGrid.Dock = DockStyle.Fill;
            clientsGrid.BackgroundColor = Color.White;
            clientsGrid.BorderStyle = BorderStyle.None;
            clientsGrid.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            clientsGrid.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            clientsGrid.MultiSelect = false;
            clientsGrid.ReadOnly = true;
            clientsGrid.AllowUserToAddRows = false;
            clientsGrid.RowHeadersVisible = false;

            clientsGrid.ColumnHeadersDefaultCellStyle.BackColor = dark;
            clientsGrid.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            clientsGrid.EnableHeadersVisualStyles = false;

            clientsGrid.Columns.Add("customer_id", "ID");
            clientsGrid.Columns.Add("full_name", "Full Name");
            clientsGrid.Columns.Add("phone", "Phone");
            clientsGrid.Columns.Add("email", "Email");
            clientsGrid.Columns.Add("gender", "Gender");



            clientsGrid.CellClick += ClientsGrid_CellClick;

            gridPanel.Controls.Add(clientsGrid);
            gridPanel.Controls.Add(searchPanel);
        }

        // ===============================
        // 6. ADD CLIENT (CREATE)
        // ===============================

        private void AddClient()
        {
            if (txtName == null ||
     txtPhone == null ||
     txtEmail == null ||
     txtAddress == null ||
    txtGender == null ||
    txtDate_of_birth == null ||
    txtAccount_number == null ||
    txtAccount_type == null ||
    txtBalance == null)
            {
                MessageBox.Show("One or more input fields are not initialized.");
                return;
            }
            if (!ValidateInputs())
                return;

            try
            {
                using var connection =
                    new NpgsqlConnection(connectionString);

                connection.Open();

                string sql = @"
                   INSERT INTO customers
                    (full_name, phone, email, gender, date_of_birth,
                    address, account_number, account_type, balance)
                    VALUES
                    (@name, @phone, @email, @gender, @date_of_birth, @address, @account_number, @account_type, @balance)";

                using var command =
                    new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue(
                    "name", txtName.Text.Trim());

                command.Parameters.AddWithValue(
                    "phone", txtPhone.Text.Trim());

                command.Parameters.AddWithValue(
                    "email", txtEmail.Text.Trim());

                command.Parameters.AddWithValue(
                    "gender", txtGender.Text.Trim());

                command.Parameters.AddWithValue(
                 "date_of_birth",
                 DateTime.Parse(txtDate_of_birth.Text.Trim()));

                command.Parameters.AddWithValue(
                    "address", txtAddress.Text.Trim());

                command.Parameters.AddWithValue(
                    "account_number", txtAccount_number.Text.Trim());

                command.Parameters.AddWithValue(
                    "account_type", txtAccount_type.Text.Trim());
                command.Parameters.AddWithValue(
                    "balance", decimal.Parse(txtBalance.Text.Trim()));

                command.ExecuteNonQuery();

                MessageBox.Show("Client added successfully!");

                ClearFields();
                LoadClients();
                lblStatus.Text = "Client added successfully.";
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // ===============================
        // 7. VIEW CLIENTS (READ)
        // ===============================

        private void LoadClients(string search = "")
        {
            if (clientsGrid == null)
                return;

            try
            {
                clientsGrid.Rows.Clear();

                using var connection =
                    new NpgsqlConnection(connectionString);

                connection.Open();

                string sql = @"
                    SELECT customer_id, full_name, phone,
                           email,gender, date_of_birth, address, account_number, account_type, balance
                    FROM customers
                    WHERE (@search = ''
                       OR full_name ILIKE @pattern
                       OR CAST(customer_id AS TEXT) ILIKE @pattern)
                    ORDER BY customer_id DESC";

                using var command =
                    new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("search", search);
                command.Parameters.AddWithValue(
                    "pattern", "%" + search + "%");

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clientsGrid.Rows.Add(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.IsDBNull(2) ? "" : reader.GetString(2),
                        reader.IsDBNull(3) ? "" : reader.GetString(3),
                        reader.IsDBNull(4) ? "" : reader.GetString(4)
                    );
                }

                lblTotalClients.Text =
                    clientsGrid.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // ===============================
        // 8. SELECT CLIENT FROM GRID
        // ===============================

        private void ClientsGrid_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                clientsGrid.Rows[e.RowIndex];

            txtName.Text = row.Cells["full_name"].Value?.ToString() ?? "";
            txtPhone.Text = row.Cells["phone"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["email"].Value?.ToString() ?? "";
            txtAddress.Text = row.Cells["address"].Value?.ToString() ?? "";
        }

        // ===============================
        // 9. UPDATE CLIENT (UPDATE)
        // ===============================

        private void UpdateClient()
        {
            if (clientsGrid.CurrentRow == null)
            {
                MessageBox.Show("Select a client first.");
                return;
            }

            if (!ValidateInputs())
                return;

            try
            {
                int id = Convert.ToInt32(
                    clientsGrid.CurrentRow.Cells["customer_id"].Value);

                using var connection =
                    new NpgsqlConnection(connectionString);

                connection.Open();

                string sql = @"
                    UPDATE customers
                    SET full_name = @name,
                        phone = @phone,
                        email = @email,
                        gender =@gender,
                        date_of_birth =@date_of_birth,
                        address = @address,
                        account_type = @account_type,
                        balance = @balance
                    WHERE customer_id = @id";

                using var command =
                    new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue(
                    "name", txtName.Text.Trim());
                command.Parameters.AddWithValue(
                    "phone", txtPhone.Text.Trim());
                command.Parameters.AddWithValue(
                    "email", txtEmail.Text.Trim());
                command.Parameters.AddWithValue(
                    "address", txtAddress.Text.Trim());

                int affected = command.ExecuteNonQuery();

                if (affected > 0)
                {
                    MessageBox.Show("Client updated successfully!");
                    ClearFields();
                    LoadClients();
                    lblStatus.Text = "Client updated successfully.";
                }
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // ===============================
        // 10. DELETE CLIENT (DELETE)
        // ===============================

        private void DeleteClient()
        {
            if (clientsGrid.CurrentRow == null)
            {
                MessageBox.Show("Select a client first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this client?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                int id = Convert.ToInt32(
                    clientsGrid.CurrentRow.Cells["customer_id"].Value);

                using var connection =
                    new NpgsqlConnection(connectionString);

                connection.Open();

                string sql =
                    "DELETE FROM clients WHERE customer_id = @id";

                using var command =
                    new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("id", id);

                command.ExecuteNonQuery();

                MessageBox.Show("Client deleted successfully!");

                ClearFields();
                LoadClients();
                lblStatus.Text = "Client deleted successfully.";
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // ===============================
        // 11. VALIDATION
        // ===============================

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Enter the client's full name.");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Enter the client's phone number.");
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();

            clientsGrid.ClearSelection();
        }

        // ===============================
        // 12. LOGOUT
        // ===============================

        private void Logout()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Exit();
        }

        // ===============================
        // 13. ERROR HANDLING
        // ===============================

        private void ShowDatabaseError(Exception ex)
        {
            MessageBox.Show(
                "Database operation failed.\n\n" + ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
