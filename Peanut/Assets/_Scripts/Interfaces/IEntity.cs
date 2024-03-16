namespace Assets._Scripts.Interfaces
{
    public interface IEntity
    {
        /// <summary>
        /// Will make sure the Entity would take damage.
        /// </summary>
        /// <param name="damage">The amount of damage that would be dealt.</param>
        void TakeDamage(float damage);

        /// <summary>
        /// Makes sure that <c>DealDamage()</c> deals damage to an entity.
        /// </summary>
        /// <returns>The amount of damage that will be dealt to it.</returns>
        float DealDamage();
        
        public void Attack();

        /// <summary>
        /// Will check if the player has died. If so, it will "kill" the entity.
        /// </summary>
        /// <returns>Either true of false based on if the Entity has died or not.</returns>
        bool HasDied();
    }
}