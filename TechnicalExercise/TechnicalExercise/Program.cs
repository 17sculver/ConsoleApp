using TechnicalExercise;

namespace TechnicalMain
{
    
    public class Program
    {
        static void Main()
        {
            ILetterService letter_service = new LetterService();
            letter_service.RunService();
        }
    }
}