using TmntCardManager.Models;

namespace TmntCardManager.Services
{
    public class CardImportValidator
    {
        public (List<Cardclass> ValidatedClasses, List<string> Warnings) ValidateAndClean(
            List<Cardclass> importedClasses)
        {
            var warnings = new List<string>();

            if (importedClasses == null || !importedClasses.Any())
            {
                return (new List<Cardclass>(), warnings);
            }

            foreach (var cardClass in importedClasses)
            {
                if (string.IsNullOrWhiteSpace(cardClass.Name))
                {
                    cardClass.Name = "Невідома Фракція";
                    warnings.Add("Знайдено фракцію без назви. Встановлено дефолтну назву.");
                }

                if (cardClass.Cards != null && cardClass.Cards.Any())
                {
                    foreach (var card in cardClass.Cards)
                    {
                        if (string.IsNullOrWhiteSpace(card.Name))
                        {
                            card.Name = "Безіменна Карта";
                            warnings.Add($"У фракції '{cardClass.Name}' була карта без назви. Виправлено.");
                        }

                        if (card.Agility < 0)
                        {
                            card.Agility = 0;
                            warnings.Add(
                                $"У фракції '{cardClass.Name}' карта '{card.Name}' мала від'ємне значення Спритності. Виправлено на 0.");
                        }

                        if (card.Wit < 0)
                        {
                            card.Wit = 0;
                            warnings.Add(
                                $"У фракції '{cardClass.Name}' карта '{card.Name}' мала від'ємне значення Кмітливості. Виправлено на 0.");
                        }

                        if (card.Skill < 0)
                        {
                            card.Skill = 0;
                            warnings.Add(
                                $"У фракції '{cardClass.Name}' карта '{card.Name}' мала від'ємне значення Майстерності. Виправлено на 0.");
                        }

                        if (card.Strength < 0)
                        {
                            card.Strength = 0;
                            warnings.Add(
                                $"У фракції '{cardClass.Name}' карта '{card.Name}' мала від'ємне значення Сили. Виправлено на 0.");
                        }

                        if (card.Imageurl == null)
                        {
                            card.Imageurl = "";
                            warnings.Add(
                                $"У фракції '{cardClass.Name}' карта '{card.Name}' мала відсутнє зображення. Встановлено пусту строку.");
                        }
                    }
                }

            }

            return (importedClasses, warnings);
        }
    }
}