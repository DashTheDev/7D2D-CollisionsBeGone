namespace CollisionsBeGone;

public static class LocalisationUtility
{
    private const string LanguageName = "English";

    public static string GetMoveAwayMessage()
    {
       return Localization.Get("moveAwayMsg", _languageName: LanguageName);
    }
}