namespace FarmTypeManager.Monsters
{
    /// <summary>An interface for subclasses of Monster that replace hardcoded damage values with a customizable value.</summary>
    public interface ICustomDamage
    {
        /// <summary>
        /// A value that will preserve and/or replace this monster's hardcoded damage values.
        /// </summary>
        int CustomDamage { get; set; }
    }
}
