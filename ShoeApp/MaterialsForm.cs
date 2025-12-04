using ShoeApp.Styles;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShoeApp
{
    public partial class MaterialsForm : Form
    {
        private string connectionString;
        private string userRole;
        private DataTable dataTable;
        private string currentTable = "";

        // Элементы интерфейса
        private DataGridView dataGridViewData;
        private ComboBox comboBoxTables;
        private ComboBox comboBoxFilterField;
        private TextBox txtFilterValue;
        private TextBox txtSearch;
        private Button btnApplyFilter;
        private Button btnClearFilter;
        private Button btnSortAsc;
        private Button btnSortDesc;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnRefresh;
        private Button btnExport;
        private Button btnBack;
        private Label lblRecordCount;
        private Label lblTitle;
        private Label lblFilter;
        private Label lblSearch;
        private Label lblTableSelect;

        public MaterialsForm(string connectionString, string userRole)
        {
            this.connectionString = connectionString;
            this.userRole = userRole;

            InitializeInterface();
            ApplyStyleToControls();
            SetupRoleBasedAccess();
            LoadTablesList();
        }

        private void InitializeInterface()
        {
            // Применить стиль к форме
            ApplyStyle.ApplyToForm(this, new Size(1200, 650));
            this.Text = $"Управление данными - {userRole}";

            // Заголовок
            lblTitle = ApplyStyle.CreateFormTitle("📊 Управление данными", new Point(20, 20));
            lblTitle.Size = new Size(400, 40);
            this.Controls.Add(lblTitle);

            // Выбор таблицы
            lblTableSelect = new Label
            {
                Text = "Выберите таблицу:",
                Location = new Point(20, 80),
                Size = new Size(120, 25)
            };
            this.Controls.Add(lblTableSelect);

            comboBoxTables = new ComboBox
            {
                Location = new Point(150, 80),
                Size = new Size(200, 25)
            };
            comboBoxTables.SelectedIndexChanged += comboBoxTables_SelectedIndexChanged;
            this.Controls.Add(comboBoxTables);

            // DataGridView для данных
            dataGridViewData = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(1140, 300),
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(dataGridViewData);

            // Панель фильтрации
            lblFilter = new Label
            {
                Text = "Фильтрация:",
                Location = new Point(20, 440),
                Size = new Size(80, 25)
            };
            this.Controls.Add(lblFilter);

            comboBoxFilterField = new ComboBox
            {
                Location = new Point(110, 440),
                Size = new Size(150, 25)
            };
            this.Controls.Add(comboBoxFilterField);

            txtFilterValue = new TextBox
            {
                Location = new Point(270, 440),
                Size = new Size(150, 25),
            };
            this.Controls.Add(txtFilterValue);

            btnApplyFilter = new Button
            {
                Text = "Применить фильтр",
                Location = new Point(430, 440),
                Size = new Size(120, 25)
            };
            btnApplyFilter.Click += BtnApplyFilter_Click;
            this.Controls.Add(btnApplyFilter);

            btnClearFilter = new Button
            {
                Text = "Сбросить фильтр",
                Location = new Point(560, 440),
                Size = new Size(120, 25)
            };
            btnClearFilter.Click += BtnClearFilter_Click;
            this.Controls.Add(btnClearFilter);

            // Кнопки сортировки
            btnSortAsc = new Button
            {
                Text = "Сортировка ↑",
                Location = new Point(690, 440),
                Size = new Size(100, 25)
            };
            btnSortAsc.Click += BtnSortAsc_Click;
            this.Controls.Add(btnSortAsc);

            btnSortDesc = new Button
            {
                Text = "Сортировка ↓",
                Location = new Point(800, 440),
                Size = new Size(100, 25)
            };
            btnSortDesc.Click += BtnSortDesc_Click;
            this.Controls.Add(btnSortDesc);

            // Панель поиска
            lblSearch = new Label
            {
                Text = "Поиск:",
                Location = new Point(20, 480),
                Size = new Size(60, 25)
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(90, 480),
                Size = new Size(200, 25),
            };
            this.Controls.Add(txtSearch);

            btnSearch = new Button
            {
                Text = "Найти",
                Location = new Point(300, 480),
                Size = new Size(80, 25)
            };
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // Статистика
            lblRecordCount = new Label
            {
                Text = "Записей: 0",
                Location = new Point(900, 440),
                Size = new Size(150, 25),
                ForeColor = ApplyStyle.TitleColor
            };
            this.Controls.Add(lblRecordCount);

            // Панель кнопок CRUD
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(20, 520),
                Size = new Size(120, 40)
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(150, 520),
                Size = new Size(140, 40)
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(300, 520),
                Size = new Size(120, 40)
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Location = new Point(430, 520),
                Size = new Size(120, 40)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(560, 520),
                Size = new Size(120, 40)
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            btnExport = new Button
            {
                Text = "📊 Экспорт в CSV",
                Location = new Point(690, 520),
                Size = new Size(140, 40)
            };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            btnBack = new Button
            {
                Text = "← Назад в меню",
                Location = new Point(1010, 520),
                Size = new Size(150, 40)
            };
            btnBack.Click += BtnBack_Click;
            this.Controls.Add(btnBack);
        }

        private void ApplyStyleToControls()
        {
            // Применить стиль к заголовку
            ApplyStyle.ApplyTitleStyle(lblTitle);

            // Применить стиль к меткам
            ApplyStyle.ApplyLabelStyle(lblTableSelect);
            ApplyStyle.ApplyLabelStyle(lblFilter);
            ApplyStyle.ApplyLabelStyle(lblSearch);
            ApplyStyle.ApplyLabelStyle(lblRecordCount);

            // Применить стиль к текстовым полям
            ApplyStyle.ApplyTextBoxStyle(txtFilterValue);
            ApplyStyle.ApplyTextBoxStyle(txtSearch);

            // Применить стиль к комбобоксам
            ApplyStyle.ApplyComboBoxStyle(comboBoxTables);
            ApplyStyle.ApplyComboBoxStyle(comboBoxFilterField);

            // Применить стиль к кнопкам действий
            ApplyStyle.ApplyMainButtonStyle(btnAdd);
            ApplyStyle.ApplyMainButtonStyle(btnEdit);
            ApplyStyle.ApplyMainButtonStyle(btnDelete);
            ApplyStyle.ApplyMainButtonStyle(btnSave);
            ApplyStyle.ApplyMainButtonStyle(btnRefresh);
            ApplyStyle.ApplyMainButtonStyle(btnExport);

            // Применить стиль к кнопкам фильтрации
            ApplyStyle.ApplySecondaryButtonStyle(btnApplyFilter);
            ApplyStyle.ApplySecondaryButtonStyle(btnClearFilter);
            ApplyStyle.ApplySecondaryButtonStyle(btnSortAsc);
            ApplyStyle.ApplySecondaryButtonStyle(btnSortDesc);
            ApplyStyle.ApplySecondaryButtonStyle(btnSearch);

            // Применить стиль к кнопке Назад
            ApplyStyle.ApplyExitButtonStyle(btnBack);

            // Применить стиль к DataGridView
            ApplyStyle.ApplyDataGridViewStyle(dataGridViewData);
        }

        private void SetupRoleBasedAccess()
        {
            // Настройка доступа в зависимости от роли
            // Исправлено: правильные русские названия ролей
            switch (userRole)
            {
                case "Админ":
                    // Полный доступ ко всем операциям
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = true;
                    break;

                case "Менеджер":
                    // Менеджер может добавлять, редактировать, сохранять, но не удалять
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = false; // Менеджер не может удалять
                    btnSave.Enabled = true;
                    break;

                case "Авторизированный клиент":
                    // Клиенту только просмотр
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                    break;
            }
        }

        private void LoadTablesList()
        {
            comboBoxTables.Items.Clear();

            // Определяем доступные таблицы в зависимости от роли
            if (userRole == "Админ" || userRole == "Менеджер" || userRole == "Авторизированный клиент")
            {
                comboBoxTables.Items.AddRange(new string[] {
                "Tovars",
                "Orders",
                "PVZ",
                "Users"
            });
            }

            if (comboBoxTables.Items.Count > 0)
            {
                comboBoxTables.SelectedIndex = 0;
            }
        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTables.SelectedItem != null)
            {
                currentTable = comboBoxTables.SelectedItem.ToString();
                LoadCurrentTable();
                SetupFilterComboBox();
            }
        }

        private void LoadCurrentTable()
        {
            if (string.IsNullOrEmpty(currentTable)) return;

            try
            {
                // Очищаем предыдущие данные
                dataGridViewData.DataSource = null;
                dataTable = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Простой SELECT без JOIN
                    string query = $"SELECT * FROM [{currentTable}]";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridViewData.DataSource = dataTable;
                    }
                }

                dataGridViewData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewData.ReadOnly = true;

                // Обновляем заголовок формы
                this.Text = $"{currentTable} - {userRole}";
                lblRecordCount.Text = $"Записей: {dataTable.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы '{currentTable}': {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupFilterComboBox()
        {
            comboBoxFilterField.Items.Clear();
            if (dataTable != null)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    comboBoxFilterField.Items.Add(column.ColumnName);
                }
                if (comboBoxFilterField.Items.Count > 0)
                    comboBoxFilterField.SelectedIndex = 0;
            }
        }

        // ФИЛЬТРАЦИЯ И СОРТИРОВКА
        private void BtnSortAsc_Click(object sender, EventArgs e)
        {
            if (dataTable != null && comboBoxFilterField.SelectedItem != null && dataGridViewData.DataSource != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = comboBoxFilterField.SelectedItem.ToString() + " ASC";
                dataGridViewData.DataSource = dataView;
            }
        }

        private void BtnSortDesc_Click(object sender, EventArgs e)
        {
            if (dataTable != null && comboBoxFilterField.SelectedItem != null && dataGridViewData.DataSource != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = comboBoxFilterField.SelectedItem.ToString() + " DESC";
                dataGridViewData.DataSource = dataView;
            }
        }

        private void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            if (dataTable != null && comboBoxFilterField.SelectedItem != null && dataGridViewData.DataSource != null)
            {
                string filterValue = txtFilterValue.Text.Trim();
                if (!string.IsNullOrEmpty(filterValue))
                {
                    string filterField = comboBoxFilterField.SelectedItem.ToString();
                    DataView dataView = new DataView(dataTable);
                    dataView.RowFilter = $"CONVERT([{filterField}], 'System.String') LIKE '%{filterValue}%'";
                    dataGridViewData.DataSource = dataView;
                }
                else
                {
                    dataGridViewData.DataSource = dataTable;
                }
                lblRecordCount.Text = $"Записей: {dataGridViewData.Rows.Count}";
            }
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            if (dataTable != null && dataGridViewData.DataSource != null)
            {
                dataGridViewData.DataSource = dataTable;
                txtFilterValue.Text = "";
                lblRecordCount.Text = $"Записей: {dataTable.Rows.Count}";
            }
        }

        // ПОИСК
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText) && dataGridViewData.Rows.Count > 0)
            {
                bool found = false;
                foreach (DataGridViewRow row in dataGridViewData.Rows)
                {
                    row.Selected = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText.ToLower()))
                        {
                            row.Selected = true;
                            dataGridViewData.FirstDisplayedScrollingRowIndex = row.Index;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found)
                {
                    MessageBox.Show("Запись не найдена.", "Поиск",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // CRUD ОПЕРАЦИИ (только для Админа и Менеджера)
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (userRole != "Админ" && userRole != "Менеджер")
            {
                MessageBox.Show("Добавление записей доступно только для администратора и менеджера.", "Доступ запрещен",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Включаем режим добавления
                dataGridViewData.ReadOnly = false;
                dataGridViewData.AllowUserToAddRows = true;

                // Добавляем новую строку в DataTable
                DataRow newRow = dataTable.NewRow();
                dataTable.Rows.Add(newRow);

                // Прокручиваем к новой строке
                int newRowIndex = dataTable.Rows.Count - 1;
                if (newRowIndex >= 0 && newRowIndex < dataGridViewData.Rows.Count)
                {
                    if (dataGridViewData.Columns.Count > 1)
                    {
                        dataGridViewData.CurrentCell = dataGridViewData.Rows[newRowIndex].Cells[1];
                    }
                    else if (dataGridViewData.Columns.Count > 0)
                    {
                        dataGridViewData.CurrentCell = dataGridViewData.Rows[newRowIndex].Cells[0];
                    }
                    dataGridViewData.BeginEdit(true);
                }

                MessageBox.Show("Введите данные новой записи. Для сохранения нажмите кнопку 'Сохранить'.",
                    "Добавление записи",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (userRole != "Админ" && userRole != "Менеджер")
            {
                MessageBox.Show("Редактирование записей доступно только для администратора и менеджера.", "Доступ запрещен",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridViewData.CurrentRow != null && !dataGridViewData.CurrentRow.IsNewRow)
            {
                // Включаем редактирование всех ячеек
                dataGridViewData.ReadOnly = false;
                dataGridViewData.AllowUserToAddRows = false;

                // Разрешаем редактирование всех столбцов
                foreach (DataGridViewColumn column in dataGridViewData.Columns)
                {
                    column.ReadOnly = false;
                }

                // Переводим текущую строку в режим редактирования
                if (dataGridViewData.Columns.Count > 1)
                {
                    dataGridViewData.CurrentCell = dataGridViewData.CurrentRow.Cells[1];
                }
                else if (dataGridViewData.Columns.Count > 0)
                {
                    dataGridViewData.CurrentCell = dataGridViewData.CurrentRow.Cells[0];
                }
                dataGridViewData.BeginEdit(true);

                MessageBox.Show("Редактирование разрешено.\nВнесите изменения и нажмите кнопку 'Сохранить'.",
                    "Редактирование",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.", "Редактирование",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (userRole != "Админ" && userRole != "Менеджер")
            {
                MessageBox.Show("Сохранение изменений доступно только для администратора и менеджера.", "Доступ запрещен",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Завершаем редактирование текущей ячейки
                if (dataGridViewData.IsCurrentCellDirty)
                {
                    dataGridViewData.EndEdit();
                }

                // Применяем все ожидающие изменения DataTable
                if (dataTable.GetChanges() != null)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Используем SqlCommandBuilder для автоматического создания команд
                        string selectQuery = $"SELECT * FROM [{currentTable}]";
                        using (SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, conn))
                        {
                            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                            int rowsAffected = adapter.Update(dataTable);

                            MessageBox.Show($"Изменения сохранены успешно.\nОбновлено записей: {rowsAffected}",
                                "Сохранение",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Обновляем данные
                            LoadCurrentTable();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Сохранение",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Возвращаем режим только для чтения
                dataGridViewData.ReadOnly = true;
                dataGridViewData.AllowUserToAddRows = false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Удаление разрешено только Админу
            if (userRole != "Админ")
            {
                MessageBox.Show("Удаление записей доступно только для администратора.", "Доступ запрещен",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridViewData.CurrentRow != null && !dataGridViewData.CurrentRow.IsNewRow)
            {
                try
                {
                    // Получаем первую колонку (предполагаем, что это первичный ключ)
                    string primaryKeyValue = "";
                    string firstColumnName = dataGridViewData.Columns[0].Name;

                    if (dataGridViewData.CurrentRow.Cells[0].Value != null)
                    {
                        primaryKeyValue = dataGridViewData.CurrentRow.Cells[0].Value.ToString();
                    }

                    if (string.IsNullOrEmpty(primaryKeyValue))
                    {
                        MessageBox.Show("Не удалось получить ID записи.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить эту запись?\nID: {primaryKeyValue}",
                        "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // Удаляем запись из базы данных
                            string deleteQuery = $"DELETE FROM [{currentTable}] WHERE [{firstColumnName}] = @Id";
                            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", primaryKeyValue);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Запись успешно удалена.", "Удаление",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Обновляем данные
                                    LoadCurrentTable();
                                }
                                else
                                {
                                    MessageBox.Show("Запись не найдена.", "Удаление",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        MessageBox.Show("Нельзя удалить запись, так как она используется в других таблицах!",
                            "Ошибка удаления",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Удаление",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ОБНОВЛЕНИЕ ДАННЫХ
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentTable();
        }

        // ЭКСПОРТ В CSV
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта.", "Экспорт",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv",
                    Title = "Экспорт данных",
                    FileName = $"{currentTable}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveDialog.FileName;
                    ExportToCSV(filePath);

                    MessageBox.Show($"Данные успешно экспортированы в файл:\n{filePath}", "Экспорт",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Заголовки
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    writer.Write($"\"{dataTable.Columns[i].ColumnName}\"");
                    if (i < dataTable.Columns.Count - 1)
                        writer.Write(";");
                }
                writer.WriteLine();

                // Данные
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        string value = row[i]?.ToString() ?? "";
                        value = value.Replace("\"", "\"\"");
                        writer.Write($"\"{value}\"");
                        if (i < dataTable.Columns.Count - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                }
            }
        }

        // НАВИГАЦИЯ
        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ЗАГРУЗКА ФОРМЫ
        private void MaterialsForm_Load(object sender, EventArgs e)
        {
            // Загружаем первую таблицу при открытии формы
            if (comboBoxTables.Items.Count > 0)
            {
                currentTable = comboBoxTables.Items[0].ToString();
                LoadCurrentTable();
            }
        }
    }
}