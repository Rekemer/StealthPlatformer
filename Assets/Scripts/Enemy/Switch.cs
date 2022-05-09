namespace Enemy
{
    public class Switch : IState
    {
        private IEnemy _enemy;
        
        public Switch(IEnemy enemy)
        {
            _enemy = enemy;
        }

        public void Tick()
        {
            _enemy.Switch();
        }

        public void OnExit()
        {
           
        }
    }
}