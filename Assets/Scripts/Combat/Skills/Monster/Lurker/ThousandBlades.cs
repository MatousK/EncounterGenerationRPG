namespace Assets.Scripts.Combat.Skills.Monster.Lurker
{
    public class ThousandBlades : Attack
    {
        public ThousandBlades()
        {
            Cooldown = 5;
            Repetitions = 10;
            Speed = 5;
            DamagePerHit = 1;
        }
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
    }
}
