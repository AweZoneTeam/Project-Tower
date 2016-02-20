/* 
 * Created by Alex 'Extravert' Kasaurov
 * From  xgm.guru community
 */
namespace XGM.GURU
{
    public class UnitInterval
    {
        public float length;
        public float value;
        public UnitInterval(float length)
        {
            this.length = length;
        }

        public float Get()
        {
            return value/length;
        }

        public float Get(float val)
        {
            return val/length;
        }

        public float Plus(float plus)
        {
            value += plus;
            return value;
        }
        public float Minus(float minus)
        {
            value -= minus;
            return value;
        }
    }
}