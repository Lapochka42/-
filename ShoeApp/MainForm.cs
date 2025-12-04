using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoeApp.Styles;

namespace ShoeApp
{
    public partial class MainForm : Form
    {
        private string connectionString;
        private string userRole;

        // Только 3 основные кнопки - в нужном порядке
        private Button btnSuppliers;
        private Button btnMaterials;
        private Button btnReturnToLogin;
        private Button btnExit;
        private Label lblWelcome;
        private Label lblLogo;
        private PictureBox logo;

        public MainForm(string connectionString, string userRole)
        {
            this.connectionString = connectionString;
            this.userRole = userRole;

            // Устанавливаем заголовок формы
            this.Text = $"Система учёта обуви - {userRole}";

            // Создаем и настраиваем интерфейс
            InitializeInterface();
            ApplyStyleToControls();
            SetupRoleBasedAccess();
        }

        private void InitializeInterface()
        {
            // Заголовок формы
            lblWelcome = ApplyStyle.CreateFormTitle($"Система учёта обуви\nРоль: {userRole}", new Point(0, 30));
            lblWelcome.Size = new Size(650, 60);
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblWelcome);

            // КНОПКА 1: Просмотр таблиц (первая по порядку)
            btnSuppliers = new Button
            {
                Text = "🏢 Просмотр таблиц",
                Size = new Size(250, 80),
                Location = new Point(50, 120),
                Visible = false,
                Tag = "suppliers"
            };
            btnSuppliers.Click += BtnSuppliers_Click;
            this.Controls.Add(btnSuppliers);

            // КНОПКА 2: Материалы (вторая по порядку)
            btnMaterials = new Button
            {
                Text = "📦 Управление данными",
                Size = new Size(250, 80),
                Location = new Point(50, 220),
                Visible = false,
                Tag = "materials"
            };
            btnMaterials.Click += BtnMaterials_Click;
            this.Controls.Add(btnMaterials);

            // Кнопка "Вернуться на форму входа"
            btnReturnToLogin = new Button
            {
                Text = "↩️ Вернуться на форму входа",
                Size = new Size(200, 40),
                Location = new Point(50, 420), // Сдвигаем левее
                Visible = true
            };
            btnReturnToLogin.Click += BtnReturnToLogin_Click;
            this.Controls.Add(btnReturnToLogin);

            // Кнопка "Выход"
            btnExit = new Button
            {
                Text = "🚪 Выход из системы",
                Size = new Size(200, 40),
                Location = new Point(260, 420), // Сдвигаем левее, рядом с первой кнопкой
                Visible = true
            };
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);

            // Добавляем логотип (если есть)
            logo = new PictureBox
            {
                Location = new Point(350, 120),
                Size = new Size(250, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            // Если есть изображение логотипа в ресурсах
            try
            {
                // logo.Image = Properties.Resources.Logo;
                this.Controls.Add(logo);
            }
            catch
            {
                // Если нет логотипа, создаем текстовый заголовок
                lblLogo = new Label
                {
                    Text = "Магазин обуви\n«FADEEV»",
                    Location = new Point(350, 150),
                    Size = new Size(250, 100),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                ApplyStyle.ApplyTitleStyle(lblLogo);
                lblLogo.Font = new Font("Comic Sans MS", 14, FontStyle.Bold);
                this.Controls.Add(lblLogo);
            }

            // Подключение обработчиков событий формы
            this.Load += MainForm_Load;
            this.Resize += MainForm_Resize;
        }

        private void ApplyStyleToControls()
        {
            // Применить стиль к форме
            ApplyStyle.ApplyToForm(this, ApplyStyle.MainFormSize);

            // Применить стиль к заголовку
            ApplyStyle.ApplyTitleStyle(lblWelcome);

            // Применить стиль к кнопкам навигации
            ApplyStyle.ApplyLargeButtonStyle(btnSuppliers);
            ApplyStyle.ApplyLargeButtonStyle(btnMaterials);

            // Применить стиль к кнопке возврата
            ApplyStyle.ApplySecondaryButtonStyle(btnReturnToLogin);
            // Применить стиль к кнопке выхода
            ApplyStyle.ApplyExitButtonStyle(btnExit);
        }

        private void SetupRoleBasedAccess()
        {
            // Настраиваем доступ в зависимости от роли
            // Исправлено: правильные русские названия ролей
            switch (userRole)
            {
                case "Админ":
                    // Администратор видит все кнопки
                    btnSuppliers.Visible = true;
                    btnMaterials.Visible = true;
                    break;

                case "Менеджер":
                    // Менеджер видит все кнопки
                    btnSuppliers.Visible = true;
                    btnMaterials.Visible = true;
                    break;

                case "Авторизированный клиент":
                    // Клиент не должен попадать сюда, но на всякий случай
                    btnSuppliers.Visible = true; // Может просматривать таблицы
                    btnMaterials.Visible = false; // Не может управлять данными
                    break;
            }
        }

        // Обработчики событий для кнопок        
        private void BtnSuppliers_Click(object sender, EventArgs e)
        {
            // Открываем форму просмотра таблиц
            using (var waitForm = new LoadingForm("Форма таблиц загружается. Пожалуйста, подождите несколько секунд"))
            {
                waitForm.Show();
                Application.DoEvents();

                UserForm userForm = new UserForm(connectionString, userRole);
                waitForm.Close();
                userForm.ShowDialog();
            }
        }

        private void BtnMaterials_Click(object sender, EventArgs e)
        {
            // Открываем форму управления данными
            using (var waitForm = new LoadingForm("Форма управления данными загружается. Пожалуйста, подождите несколько секунд"))
            {
                waitForm.Show();
                Application.DoEvents();

                MaterialsForm materialsForm = new MaterialsForm(connectionString, userRole);
                waitForm.Close();
                materialsForm.ShowDialog();
            }
        }

        private void BtnReturnToLogin_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите вернуться на форму входа?\n\n",
                "Подтверждение возврата",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Hide();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        public class LoadingForm : Form
        {
            public LoadingForm(string message)
            {
                InitializeLoadingForm(message);
            }

            private void InitializeLoadingForm(string message)
            {
                this.Text = "Загрузка";
                this.Size = new Size(400, 150);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.FormBorderStyle = FormBorderStyle.None;
                this.BackColor = Color.FromArgb(0xAB, 0xCF, 0xCE);
                this.FormBorderStyle = FormBorderStyle.FixedSingle;

                var label = new Label
                {
                    Text = message,
                    Location = new Point(50, 50),
                    Size = new Size(300, 50),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Comic Sans MS", 10)
                };

                this.Controls.Add(label);
            }
        }

        // Обработчик загрузки формы
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Центрируем заголовок
            lblWelcome.Location = new Point(
                (this.ClientSize.Width - lblWelcome.Width) / 2,
                30
            );

            // Центрируем логотип/текст
            if (lblLogo != null)
            {
                lblLogo.Location = new Point(
                    (this.ClientSize.Width - lblLogo.Width) / 2,
                    150
                );
            }

            if (logo != null)
            {
                logo.Location = new Point(
                    (this.ClientSize.Width - logo.Width) / 2,
                    120
                );
            }

            // Центрируем кнопки слева
            int leftMargin = 50;
            btnSuppliers.Location = new Point(leftMargin, 120);
            btnMaterials.Location = new Point(leftMargin, 220);

            // Фиксируем позиции кнопок внизу слева
            btnReturnToLogin.Location = new Point(
                leftMargin + 300,
                this.ClientSize.Height - 150
            );

            btnExit.Location = new Point(
                leftMargin + 300, // Размещаем рядом с первой кнопкой
                this.ClientSize.Height - 80
            );
        }

        // Обработчик изменения размера формы
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Пересчитываем позиции при изменении размера
            if (btnReturnToLogin != null)
            {
                btnReturnToLogin.Location = new Point(
                    50, // Фиксированная позиция слева
                    this.ClientSize.Height - 80
                );
            }

            if (btnExit != null)
            {
                btnExit.Location = new Point(
                    260, // Фиксированная позиция слева, рядом с кнопкой возврата
                    this.ClientSize.Height - 80
                );
            }
        }
    }
}