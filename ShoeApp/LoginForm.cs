using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoeApp.Styles;

namespace ShoeApp
{
    public partial class LoginForm : Form
    {
        private string connectionString;

        public LoginForm()
        {
            InitializeComponent();
            ApplyStyleToControls();
            connectionString = @"Data Source=ADCLG1;Initial Catalog=ПДЭ_обувь_Назимов;Integrated Security=True";
        }

        private void ApplyStyleToControls()
        {
            // Применить стиль к форме
            ApplyStyle.ApplyToForm(this, new Size(450, 400));
            this.Text = "Авторизация - Система учёта обуви";

            // Применить стиль к заголовку
            ApplyStyle.ApplyTitleStyle(label1);
            label1.Text = "Вход в систему";
            label1.Location = new Point(50, 30);
            label1.Size = new Size(350, 40);
            label1.TextAlign = ContentAlignment.MiddleCenter;

            // Применить стиль к меткам
            ApplyStyle.ApplyLabelStyle(label2); // Логин
            ApplyStyle.ApplyLabelStyle(label3); // Пароль

            // Обновляем текст меток
            label2.Text = "Логин (email):";
            label3.Text = "Пароль:";

            // Применить стиль к текстовым полям
            ApplyStyle.ApplyTextBoxStyle(txtUsername);
            ApplyStyle.ApplyTextBoxStyle(txtPassword);

            // Применить стиль к кнопкам
            ApplyStyle.ApplyMainButtonStyle(btnLogin);
            ApplyStyle.ApplyExitButtonStyle(btnExit);

            // Настроить позиционирование элементов
            RepositionControls();
        }

        private void RepositionControls()
        {
            // Центрируем элементы
            int centerX = (this.ClientSize.Width - 300) / 2;

            label1.Location = new Point(centerX, 40);

            label2.Location = new Point(centerX, 100);
            txtUsername.Location = new Point(centerX, 125);
            txtUsername.Size = new Size(300, 25);

            label3.Location = new Point(centerX, 165);
            txtPassword.Location = new Point(centerX, 190);
            txtPassword.Size = new Size(300, 25);

            btnLogin.Location = new Point(centerX, 240);
            btnLogin.Size = new Size(300, 40);

            btnExit.Location = new Point(centerX, 300);
            btnExit.Size = new Size(300, 40);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка ввода",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Проверяем учетные данные в базе данных
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ищем пользователя по логину и паролю
                    string query = @"
                        SELECT [Роль_сотрудника], [ФИО] 
                        FROM Users 
                        WHERE [Логин] = @Login AND [Пароль] = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Учетные данные верны
                                string role = reader["Роль_сотрудника"].ToString();
                                string fullName = reader["ФИО"].ToString();

                                this.Hide();

                                Form nextForm = null;

                                // Исправлено: правильное распределение по ролям
                                if (role == "Авторизированный клиент")
                                {
                                    // Для клиента открываем UserForm
                                    nextForm = new UserForm(connectionString, role);
                                }
                                else if (role == "Админ" || role == "Менеджер")
                                {
                                    // Для админа и менеджера - MainForm
                                    nextForm = new MainForm(connectionString, role);
                                }
                                else
                                {
                                    // Для других ролей (если будут добавлены)
                                    nextForm = new MaterialsForm(connectionString, role);
                                }

                                if (nextForm != null)
                                {
                                    nextForm.FormClosed += (s, args) => this.Close();
                                    nextForm.Show();
                                }
                            }
                            else
                            {
                                // Неверные учетные данные
                                MessageBox.Show("Неверный логин или пароль.\n\nПожалуйста, проверьте введенные данные.",
                                    "Ошибка авторизации",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных:\n{ex.Message}",
                    "Ошибка базы данных",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Добавляем обработчик нажатия Enter в текстовых полях
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Обработчик изменения размера формы
        private void LoginForm_Resize(object sender, EventArgs e)
        {
            RepositionControls();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Устанавливаем фокус на поле ввода логина
            txtUsername.Focus();
        }
    }
}