namespace Assets._Scripts.Interfaces
{
    public interface IEntity
    {
        /// <summary>
        /// Will make sure the Entity would take damage.
        /// </summary>
        /// <param name="damage">The amount of damage that would be dealt.</param>
        void TakeDamage(float damage);
        
        public void Attack();

        /// <summary>
        /// Will check if the player has died. If so, it will "kill" the entity.
        /// </summary>
        /// <returns>Either true of false based on if the Entity has died or not.</returns>
        bool HasDied();
    }
}