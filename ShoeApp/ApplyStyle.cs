using System.Drawing;
using System.Windows.Forms;

namespace ShoeApp.Styles
{
    public static class ApplyStyle
    {
        // Цветовая палитра
        public static Color MainButtonColor => ColorTranslator.FromHtml("#00FA9A");
        public static Color MainButtonTextColor => Color.Black;
        public static Color SecondaryButtonColor => ColorTranslator.FromHtml("#7FFF00");
        public static Color SecondaryButtonTextColor => Color.Black;
        public static Color BackgroundColor => Color.White;
        public static Color ExitButtonColor => Color.LightGray;
        public static Color ExitButtonTextColor => Color.Black;
        public static Color TitleColor => ColorTranslator.FromHtml("#000");
        public static Color GridColor => ColorTranslator.FromHtml("#7FFF00");

        // Шрифты
        public static Font MainFont => new Font("Times New Roman", 9);
        public static Font TitleFont => new Font("Times New Roman", 16, FontStyle.Bold);
        public static Font ButtonFont => new Font("Times New Roman", 10);
        public static Font LargeButtonFont => new Font("Times New Roman", 12);

        // Размеры форм
        public static Size MainFormSize => new Size(700, 500);
        public static Size MaterialsFormSize => new Size(1200, 700);

        // Применить стиль к форме
        public static void ApplyToForm(Form form, Size? size = null)
        {
            form.BackColor = BackgroundColor;
            form.Font = MainFont;

            if (size.HasValue)
            {
                form.Size = size.Value;
            }

            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
        }

        // Применить стиль к основной кнопке
        public static void ApplyMainButtonStyle(Button button, bool withHoverEffects = true)
        {
            button.BackColor = MainButtonColor;
            button.ForeColor = MainButtonTextColor;
            button.Font = ButtonFont;
            ApplyCommonButtonStyle(button, withHoverEffects);
        }

        // Применить стиль к второстепенной кнопке
        public static void ApplySecondaryButtonStyle(Button button)
        {
            button.BackColor = SecondaryButtonColor;
            button.ForeColor = SecondaryButtonTextColor;
            button.Font = MainFont;
            ApplyCommonButtonStyle(button, false);
        }

        // Применить стиль к кнопке выхода/назад
        public static void ApplyExitButtonStyle(Button button)
        {
            button.BackColor = ExitButtonColor;
            button.ForeColor = ExitButtonTextColor;
            button.Font = MainFont;
            ApplyCommonButtonStyle(button);
        }

        // Общий стиль для всех кнопок
        private static void ApplyCommonButtonStyle(Button button, bool withHoverEffects = true)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            if (withHoverEffects)
            {
                button.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#006400"); // Темно-зеленый
                button.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#004d00"); // Еще более темный зеленый
            }
        }

        // Применить стиль к заголовку
        public static void ApplyTitleStyle(Label label)
        {
            label.Font = TitleFont;
            label.ForeColor = TitleColor;
        }

        // Применить стиль к DataGridView
        public static void ApplyDataGridViewStyle(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = BackgroundColor;
            dataGridView.GridColor = GridColor;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = MainButtonColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = MainButtonTextColor;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.DefaultCellStyle.Font = MainFont;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BorderStyle = BorderStyle.FixedSingle;
        }

        // Применить стиль к большой кнопке (для MainForm)
        public static void ApplyLargeButtonStyle(Button button)
        {
            button.BackColor = MainButtonColor;
            button.ForeColor = MainButtonTextColor;
            button.Font = LargeButtonFont;
            ApplyCommonButtonStyle(button);
        }

        // Применить стиль к текстовому полю
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = MainFont;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        // Применить стиль к комбобоксу
        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.Font = MainFont;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Применить стиль к метке
        public static void ApplyLabelStyle(Label label)
        {
            label.Font = MainFont;
        }

        // Создать заголовок формы
        public static Label CreateFormTitle(string titleText, Point location)
        {
            return new Label
            {
                Text = titleText,
                Location = location,
                Size = new Size(400, 40),
                Font = TitleFont,
                ForeColor = TitleColor
            };
        }
    }
}