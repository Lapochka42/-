using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShoeApp.Styles;

namespace ShoeApp
{
    public partial class MaterialsManagementForm : Form
    {
        private string connectionString;
        private string userRole;
        private DataTable dataTable;
        private SqlDataAdapter adapter;
        private SqlCommandBuilder commandBuilder;

        // Элементы интерфейса
        private DataGridView dataGridViewMaterials;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnRefresh;
        private Button btnBack;
        private Label lblTitle;
        private Label lblRecordCount;

        public MaterialsManagementForm(string connectionString, string userRole)
        {
            this.connectionString = connectionString;
            this.userRole = userRole;

            InitializeInterface();
            ApplyStyleToControls();
            SetupRoleBasedAccess();
            LoadMaterialsData();
        }

        private void InitializeInterface()
        {
            // Применить стиль к форме
            ApplyStyle.ApplyToForm(this, new Size(1200, 600));
            this.Text = $"Управление материалами - {userRole}";

            // Заголовок
            lblTitle = ApplyStyle.CreateFormTitle("📦 Управление материалами", new Point(20, 20));
            lblTitle.Size = new Size(500, 40);
            this.Controls.Add(lblTitle);

            // DataGridView для материалов
            dataGridViewMaterials = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(1140, 350),
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dataGridViewMaterials.CellDoubleClick += DataGridViewMaterials_CellDoubleClick;
            this.Controls.Add(dataGridViewMaterials);

            // Статистика
            lblRecordCount = new Label
            {
                Text = "Записей: 0",
                Location = new Point(20, 430),
                Size = new Size(150, 25),
                ForeColor = ApplyStyle.TitleColor
            };
            this.Controls.Add(lblRecordCount);

            // Панель кнопок CRUD
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(20, 470),
                Size = new Size(120, 40)
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(150, 470),
                Size = new Size(140, 40)
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(300, 470),
                Size = new Size(120, 40)
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Location = new Point(430, 470),
                Size = new Size(120, 40)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(560, 470),
                Size = new Size(120, 40)
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            btnBack = new Button
            {
                Text = "← Назад",
                Location = new Point(1040, 470),
                Size = new Size(120, 40)
            };
            btnBack.Click += BtnBack_Click;
            this.Controls.Add(btnBack);
        }

        private void ApplyStyleToControls()
        {
            // Применить стиль к заголовку
            ApplyStyle.ApplyTitleStyle(lblTitle);

            // Применить стиль к метке
            ApplyStyle.ApplyLabelStyle(lblRecordCount);

            // Применить стиль к кнопкам CRUD
            ApplyStyle.ApplyMainButtonStyle(btnAdd);
            ApplyStyle.ApplyMainButtonStyle(btnEdit);
            ApplyStyle.ApplyMainButtonStyle(btnDelete);
            ApplyStyle.ApplyMainButtonStyle(btnSave);
            ApplyStyle.ApplyMainButtonStyle(btnRefresh);

            // Применить стиль к кнопке Назад
            ApplyStyle.ApplyExitButtonStyle(btnBack);

            // Применить стиль к DataGridView
            ApplyStyle.ApplyDataGridViewStyle(dataGridViewMaterials);
        }

        private void SetupRoleBasedAccess()
        {
            // Настройка доступа в зависимости от роли
            switch (userRole)
            {
                case "Админ":
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = true;
                    break;

                case "Менеджер":
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = true;
                    break;

                case "Технолог":
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                    break;
            }
        }

        private void LoadMaterialsData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Берем ТОЛЬКО таблицу MATERIALS без JOIN
                    string query = "SELECT * FROM MATERIALS ORDER BY material_id";

                    adapter = new SqlDataAdapter(query, conn);
                    commandBuilder = new SqlCommandBuilder(adapter);

                    dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Настраиваем DataGridView
                    dataGridViewMaterials.DataSource = dataTable;
                    dataGridViewMaterials.ReadOnly = true; // По умолчанию только чтение

                    // Настраиваем столбцы для удобства
                    if (dataGridViewMaterials.Columns.Contains("material_id"))
                    {
                        dataGridViewMaterials.Columns["material_id"].ReadOnly = true;
                    }
                    if (dataGridViewMaterials.Columns.Contains("name"))
                    {
                        dataGridViewMaterials.Columns["name"].HeaderText = "Название";
                    }
                    if (dataGridViewMaterials.Columns.Contains("material_type_id"))
                    {
                        dataGridViewMaterials.Columns["material_type_id"].HeaderText = "ID типа";
                    }
                    if (dataGridViewMaterials.Columns.Contains("current_quantity"))
                    {
                        dataGridViewMaterials.Columns["current_quantity"].HeaderText = "Количество";
                    }
                    if (dataGridViewMaterials.Columns.Contains("unit"))
                    {
                        dataGridViewMaterials.Columns["unit"].HeaderText = "Единица";
                    }
                    if (dataGridViewMaterials.Columns.Contains("package_quantity"))
                    {
                        dataGridViewMaterials.Columns["package_quantity"].HeaderText = "В упаковке";
                    }
                    if (dataGridViewMaterials.Columns.Contains("min_quantity"))
                    {
                        dataGridViewMaterials.Columns["min_quantity"].HeaderText = "Мин. количество";
                    }
                    if (dataGridViewMaterials.Columns.Contains("price"))
                    {
                        dataGridViewMaterials.Columns["price"].HeaderText = "Цена";
                        dataGridViewMaterials.Columns["price"].DefaultCellStyle.Format = "N2";
                    }
                    if (dataGridViewMaterials.Columns.Contains("supplier_id"))
                    {
                        dataGridViewMaterials.Columns["supplier_id"].HeaderText = "ID поставщика";
                    }

                    lblRecordCount.Text = $"Записей: {dataTable.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ДОБАВЛЕНИЕ
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Включаем режим добавления
                dataGridViewMaterials.ReadOnly = false;
                dataGridViewMaterials.AllowUserToAddRows = true;

                // Добавляем новую строку в DataTable
                DataRow newRow = dataTable.NewRow();

                // Устанавливаем значения по умолчанию
                newRow["name"] = "Новый материал";
                newRow["current_quantity"] = 0;
                newRow["unit"] = "шт";
                newRow["package_quantity"] = 1;
                newRow["min_quantity"] = 0;
                newRow["price"] = 0;

                dataTable.Rows.Add(newRow);

                // Прокручиваем к новой строке
                int newRowIndex = dataTable.Rows.Count - 1;
                if (newRowIndex >= 0 && newRowIndex < dataGridViewMaterials.Rows.Count)
                {
                    dataGridViewMaterials.CurrentCell = dataGridViewMaterials.Rows[newRowIndex].Cells["name"];
                    dataGridViewMaterials.BeginEdit(true);
                }

                MessageBox.Show("Введите данные нового материала. Для сохранения нажмите кнопку 'Сохранить'.",
                    "Добавление материала",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // РЕДАКТИРОВАНИЕ
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewMaterials.CurrentRow != null && !dataGridViewMaterials.CurrentRow.IsNewRow)
            {
                // Включаем редактирование всех ячеек
                dataGridViewMaterials.ReadOnly = false;
                dataGridViewMaterials.AllowUserToAddRows = false;

                // Разрешаем редактирование всех столбцов
                foreach (DataGridViewColumn column in dataGridViewMaterials.Columns)
                {
                    // Только столбец ID оставляем нередактируемым
                    if (column.Name != "material_id")
                    {
                        column.ReadOnly = false;
                    }
                }

                // Переводим текущую строку в режим редактирования
                dataGridViewMaterials.CurrentCell = dataGridViewMaterials.CurrentRow.Cells[1]; // Вторая колонка (название)
                dataGridViewMaterials.BeginEdit(true);

                MessageBox.Show("Редактирование разрешено для всех полей.\nВнесите изменения и нажмите кнопку 'Сохранить'.",
                    "Редактирование",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите материал для редактирования.", "Редактирование",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // СОХРАНЕНИЕ
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Завершаем редактирование текущей ячейки
                if (dataGridViewMaterials.IsCurrentCellDirty)
                {
                    dataGridViewMaterials.EndEdit();
                }

                // Применяем все ожидающие изменения DataTable
                if (dataTable.GetChanges() != null)
                {
                    // Валидация данных
                    bool validationPassed = true;
                    string validationMessage = "";

                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                        {
                            // Проверка обязательного поля "name"
                            if (row["name"] == DBNull.Value || string.IsNullOrEmpty(row["name"].ToString()))
                            {
                                validationPassed = false;
                                validationMessage += "Не заполнено наименование материала.\n";
                            }

                            // Проверка цены
                            if (row["price"] != DBNull.Value)
                            {
                                decimal price = Convert.ToDecimal(row["price"]);
                                if (price < 0)
                                {
                                    validationPassed = false;
                                    validationMessage += $"Материал '{row["name"]}': цена не может быть отрицательной.\n";
                                }
                            }

                            // Проверка минимального количества
                            if (row["min_quantity"] != DBNull.Value)
                            {
                                decimal minQty = Convert.ToDecimal(row["min_quantity"]);
                                if (minQty < 0)
                                {
                                    validationPassed = false;
                                    validationMessage += $"Материал '{row["name"]}': минимальное количество не может быть отрицательным.\n";
                                }
                            }
                        }
                    }

                    if (!validationPassed)
                    {
                        MessageBox.Show($"Обнаружены ошибки:\n\n{validationMessage}\nИсправьте ошибки и попробуйте снова.",
                            "Ошибка валидации",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Сохраняем изменения в БД с новым подключением
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM MATERIALS";

                        using (SqlDataAdapter updateAdapter = new SqlDataAdapter(query, conn))
                        {
                            SqlCommandBuilder builder = new SqlCommandBuilder(updateAdapter);
                            int rowsAffected = updateAdapter.Update(dataTable);

                            MessageBox.Show($"Изменения сохранены успешно.\nОбновлено записей: {rowsAffected}",
                                "Сохранение",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Обновляем данные и пересоздаем адаптер
                            LoadMaterialsData();
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
                dataGridViewMaterials.ReadOnly = true;
                dataGridViewMaterials.AllowUserToAddRows = false;

                // Отключаем редактирование всех столбцов
                foreach (DataGridViewColumn column in dataGridViewMaterials.Columns)
                {
                    column.ReadOnly = true;
                }
            }
        }

        // УДАЛЕНИЕ
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewMaterials.CurrentRow != null && !dataGridViewMaterials.CurrentRow.IsNewRow)
            {
                try
                {
                    // Получаем ID материала - используем имя столбца material_id
                    int rowIndex = dataGridViewMaterials.CurrentRow.Index;

                    // ОТЛАДКА: Выведем все имена столбцов для проверки
                    Console.WriteLine("Доступные столбцы в DataTable:");
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Console.WriteLine($"- {column.ColumnName} (Тип: {column.DataType})");
                    }

                    // Способ 1: Попробуем получить ID через DataGridView (более надежно)
                    DataGridViewRow selectedRow = dataGridViewMaterials.CurrentRow;

                    // Проверяем разные возможные имена столбцов
                    string materialIdValue = null;
                    string materialName = "";

                    if (selectedRow.Cells["material_id"]?.Value != null)
                    {
                        materialIdValue = selectedRow.Cells["material_id"].Value.ToString();
                    }
                    else if (selectedRow.Cells["ID"]?.Value != null) // Возможно отображается как ID
                    {
                        materialIdValue = selectedRow.Cells["ID"].Value.ToString();
                    }
                    else if (dataGridViewMaterials.Columns.Contains("material_id") &&
                             selectedRow.Cells["material_id"]?.Value != null)
                    {
                        materialIdValue = selectedRow.Cells["material_id"].Value.ToString();
                    }

                    // Получаем название материала
                    if (selectedRow.Cells["name"]?.Value != null)
                    {
                        materialName = selectedRow.Cells["name"].Value.ToString();
                    }
                    else if (selectedRow.Cells["Название"]?.Value != null) // Если заголовок изменен
                    {
                        materialName = selectedRow.Cells["Название"].Value.ToString();
                    }

                    if (string.IsNullOrEmpty(materialIdValue))
                    {
                        MessageBox.Show("Ошибка: не удалось получить ID материала.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int materialId = Convert.ToInt32(materialIdValue);

                    DialogResult result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить материал?\n\n" +
                        $"Название: {materialName}\n" +
                        $"ID: {materialId}",
                        "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // Проверка на использование материала
                            string checkQuery = @"
                SELECT COUNT(*) FROM PRODUCTS WHERE material_id = @MaterialId
                UNION ALL
                SELECT COUNT(*) FROM ORDER_DETAILS WHERE material_id = @MaterialId";

                            bool hasReferences = false;
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@MaterialId", materialId);
                                using (SqlDataReader reader = checkCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.GetInt32(0) > 0)
                                        {
                                            hasReferences = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (hasReferences)
                            {
                                MessageBox.Show("Нельзя удалить материал, так как он используется в других таблицах!",
                                    "Ошибка удаления",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Способ удаления через SQL запрос напрямую
                            string deleteQuery = "DELETE FROM MATERIALS WHERE material_id = @MaterialId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@MaterialId", materialId);
                                int rowsAffected = deleteCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Материал успешно удален.", "Удаление",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Обновляем DataTable - удаляем строку по индексу
                                    if (rowIndex >= 0 && rowIndex < dataTable.Rows.Count)
                                    {
                                        dataTable.Rows[rowIndex].Delete();
                                        dataTable.AcceptChanges();
                                    }

                                    // Перезагружаем данные
                                    LoadMaterialsData();
                                }
                                else
                                {
                                    MessageBox.Show("Материал не найден или уже удален.", "Удаление",
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
                        MessageBox.Show("Нельзя удалить материал, так как он используется в других таблицах!",
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
                MessageBox.Show("Выберите материал для удаления.", "Удаление",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DataGridViewMaterials_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dataGridViewMaterials.Rows[e.RowIndex].IsNewRow)
            {
                BtnEdit_Click(sender, e);
            }
        }

        // ОБНОВЛЕНИЕ
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadMaterialsData();
        }

        // НАЗАД
        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ЗАГРУЗКА ФОРМЫ
        private void MaterialsManagementForm_Load(object sender, EventArgs e)
        {
            LoadMaterialsData();
        }
    }
}