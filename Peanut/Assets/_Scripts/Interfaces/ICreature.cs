namespace Assets._Scripts.Interfaces
{
    /// <summary>
    /// The interface for all the creatures, this would include <c>TakeDamage()</c> and <c>Die()</c> as examples.
    /// </summary>
    public interface ICreature
    {
        /// <summary>
        /// Will make sure the Creature would take damage.
        /// </summary>
        void TakeDamage();

        /// <summary>
        /// Will check if the player has died. If so, it will "kill" the Creature.
        /// </summary>
        void Die();
    }
}
