using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RPGCreator
{
    public partial class MainWindow : Window
    {
        // Ограничение очков навыков
        private int maxSkillPoints = 10;
        private Dictionary<CheckBox, int> skillCosts = new Dictionary<CheckBox, int>();

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация стоимости навыков
            InitializeSkillCosts();
        }

        // Инициализация стоимости навыков
        private void InitializeSkillCosts()
        {
            skillCosts[SmithingCheck] = 3;
            skillCosts[AlchemyCheck] = 4;
            skillCosts[PotionCheck] = 4;
            skillCosts[LockpickCheck] = 2;
            skillCosts[StealthCheck] = 3;

            // Подписываемся на события изменения CheckBox
            SmithingCheck.Checked += SkillCheck_Changed;
            SmithingCheck.Unchecked += SkillCheck_Changed;
            AlchemyCheck.Checked += SkillCheck_Changed;
            AlchemyCheck.Unchecked += SkillCheck_Changed;
            PotionCheck.Checked += SkillCheck_Changed;
            PotionCheck.Unchecked += SkillCheck_Changed;
            LockpickCheck.Checked += SkillCheck_Changed;
            LockpickCheck.Unchecked += SkillCheck_Changed;
            StealthCheck.Checked += SkillCheck_Changed;
            StealthCheck.Unchecked += SkillCheck_Changed;
        }

        // Проверка суммы очков при выборе навыков
        private void SkillCheck_Changed(object sender, RoutedEventArgs e)
        {
            int totalCost = skillCosts.Where(kvp => kvp.Key.IsChecked == true)
                                      .Sum(kvp => kvp.Value);

            // Обновляем отображение оставшихся очков
            int pointsLeft = maxSkillPoints - totalCost;
            PointsLeftText.Text = $"Осталось очков: {pointsLeft}";

            if (pointsLeft < 0)
            {
                PointsLeftText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                PointsLeftText.Foreground = System.Windows.Media.Brushes.Blue;
            }

            if (totalCost > maxSkillPoints)
            {
                ((CheckBox)sender).IsChecked = false;
                MessageBox.Show($"Превышен лимит очков навыков! (макс. {maxSkillPoints} очков)\n" +
                              $"Выбранные навыки: {totalCost} очков",
                              "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Кнопка "Создать персонажа"
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем имя
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите имя персонажа!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем сумму очков
            int totalCost = skillCosts.Where(kvp => kvp.Key.IsChecked == true)
                                      .Sum(kvp => kvp.Value);

            if (totalCost > maxSkillPoints)
            {
                MessageBox.Show($"Превышен лимит очков навыков! ({totalCost}/{maxSkillPoints})\n" +
                              "Снимите лишние навыки.",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Определяем класс
            string characterClass = GetSelectedClass();

            // Получаем выбранные навыки
            List<string> skills = GetSelectedSkills();

            // Формируем результат
            string result = "╔════════════════════════════════════╗\n";
            result += "║        ✨ ПЕРСОНАЖ СОЗДАН ✨         ║\n";
            result += "╚════════════════════════════════════╝\n\n";
            result += $"👤 Имя: {name}\n";
            result += $"⚔️ Класс: {characterClass}\n";
            result += $"💰 Очки навыков: {totalCost}/{maxSkillPoints}\n\n";

            if (skills.Count > 0)
            {
                result += "📚 НАВЫКИ:\n";
                foreach (string skill in skills)
                {
                    // Добавляем стоимость к каждому навыку
                    int cost = 0;
                    if (skill == "Кузнечное дело") cost = 3;
                    else if (skill == "Алхимия") cost = 4;
                    else if (skill == "Зельеварение") cost = 4;
                    else if (skill == "Взлом замков") cost = 2;
                    else if (skill == "Скрытность") cost = 3;
                    result += $"   • {skill} ({cost} очк.)\n";
                }
            }
            else
            {
                result += "📚 Навыки: не выбраны\n";
            }

            ResultTextBox.Text = result;
        }

        // Кнопка "Сбросить"
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем имя
            NameTextBox.Text = string.Empty;

            // Сбрасываем класс на первый (Воин)
            WarriorRadio.IsChecked = true;
            MageRadio.IsChecked = false;
            ArcherRadio.IsChecked = false;
            RogueRadio.IsChecked = false;

            // Снимаем все галочки с навыков
            SmithingCheck.IsChecked = false;
            AlchemyCheck.IsChecked = false;
            PotionCheck.IsChecked = false;
            LockpickCheck.IsChecked = false;
            StealthCheck.IsChecked = false;

            // Сбрасываем отображение очков
            PointsLeftText.Text = "Осталось очков: 10";
            PointsLeftText.Foreground = System.Windows.Media.Brushes.Blue;

            // Очищаем результат
            ResultTextBox.Text = string.Empty;
        }

        // Кнопка "Случайный выбор"
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            // Случайное имя
            string[] names = { "Артур", "Мерлин", "Леголас", "Арагорн", "Гэндальф",
                              "Торин", "Боромир", "Фарамир", "Эовин", "Гимли" };
            NameTextBox.Text = names[random.Next(names.Length)];

            // Случайный класс
            RadioButton[] classRadios = { WarriorRadio, MageRadio, ArcherRadio, RogueRadio };
            foreach (var radio in classRadios)
                radio.IsChecked = false;
            classRadios[random.Next(classRadios.Length)].IsChecked = true;

            // Случайные навыки с учётом лимита очков
            CheckBox[] skillChecks = { SmithingCheck, AlchemyCheck, PotionCheck,
                                       LockpickCheck, StealthCheck };

            // Сначала сбрасываем все
            foreach (var check in skillChecks)
                check.IsChecked = false;

            // Создаём список навыков с их стоимостью
            var skillsWithCost = new List<(CheckBox check, int cost)>
            {
                (SmithingCheck, 3),
                (AlchemyCheck, 4),
                (PotionCheck, 4),
                (LockpickCheck, 2),
                (StealthCheck, 3)
            };

            // Перемешиваем список
            var shuffled = skillsWithCost.OrderBy(x => random.Next()).ToList();

            int totalCost = 0;
            foreach (var skill in shuffled)
            {
                if (totalCost + skill.cost <= maxSkillPoints)
                {
                    skill.check.IsChecked = true;
                    totalCost += skill.cost;
                }
            }
        }

        // Получить выбранный класс
        private string GetSelectedClass()
        {
            if (WarriorRadio.IsChecked == true) return "Воин";
            if (MageRadio.IsChecked == true) return "Маг";
            if (ArcherRadio.IsChecked == true) return "Лучник";
            if (RogueRadio.IsChecked == true) return "Вор";
            return "Не выбран";
        }

        // Получить выбранные навыки
        private List<string> GetSelectedSkills()
        {
            List<string> skills = new List<string>();
            if (SmithingCheck.IsChecked == true) skills.Add("Кузнечное дело");
            if (AlchemyCheck.IsChecked == true) skills.Add("Алхимия");
            if (PotionCheck.IsChecked == true) skills.Add("Зельеварение");
            if (LockpickCheck.IsChecked == true) skills.Add("Взлом замков");
            if (StealthCheck.IsChecked == true) skills.Add("Скрытность");
            return skills;
        }
    }
}