namespace LLama.Unittest;

/// <summary>
/// Extend Fact attributes to know if we are running on release or debug. The assumption is that on CI we run on Release
/// </summary>
public class IgnoreOnCiFact : FactAttribute
{
    public IgnoreOnCiFact() {
        if( IsRelease()) {
            Skip = "Ignore on CI";
        }
    }

    private static bool IsRelease()
    {
        #if DEBUG
            return false;
        #else
            return true;
        #endif
    }
}