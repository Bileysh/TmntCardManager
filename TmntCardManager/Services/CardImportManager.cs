using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Services
{
    public class CardImportManager
    {
        private readonly TmntCardsDbContext _context;
        private readonly CardImportValidator _validator;

        public CardImportManager(TmntCardsDbContext context, CardImportValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ImportResult> ProcessAndSaveAsync(List<Cardclass> rawClasses, CancellationToken token)
        {
            var result = new ImportResult();

            var (cleanedClasses, warnings) = _validator.ValidateAndClean(rawClasses);

            if (!cleanedClasses.Any())
            {
                result.ErrorMessage = "Файл порожній або не містить жодної фракції.";
                return result;
            }

            int classesCount = 0;
            int cardsCount = 0;

            foreach (var cardClass in cleanedClasses)
            {
                var existingFraction = await _context.Cardclasses
                    .Include(c => c.Cards)
                    .FirstOrDefaultAsync(c => c.Name == cardClass.Name, token);

                if (existingFraction == null)
                {
                    _context.Cardclasses.Add(cardClass);
                    classesCount++;
                    cardsCount += cardClass.Cards?.Count ?? 0;
                }
                else
                {
                    classesCount++;
                    
                    if (cardClass.Cards != null)
                    {
                        foreach (var card in cardClass.Cards)
                        {
                            var existingCard = existingFraction.Cards.FirstOrDefault(c => c.Name == card.Name);
                            if (existingCard == null)
                            {
                                existingFraction.Cards.Add(card);
                                cardsCount++;
                            }
                            else
                            {
                                existingCard.Strength = card.Strength;
                                existingCard.Agility = card.Agility;
                                existingCard.Skill = card.Skill;
                                existingCard.Wit = card.Wit;
                                existingCard.Imageurl = card.Imageurl;
                                
                                cardsCount++;
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(token);

            if (warnings.Any())
            {
                var displayWarnings = string.Join(" ", warnings.Take(3));
                if (warnings.Count > 3) displayWarnings += " (та інші виправлення).";
                
                result.WarningMessage = $"Оброблено {classesCount} фракцій та {cardsCount} карт. Виправлення: {displayWarnings}";
            }
            else
            {
                result.SuccessMessage = $"Успішно оброблено {classesCount} фракцій та {cardsCount} карт без помилок!";
            }

            return result;
        }
    }
}