using System.Numerics;

namespace Shared
{
    public static class FrameData
    {
        public static Vector2 GetNextFrameData(
            NetworkingData.PlayerInputData input,
            NetworkingData.PlayerStateData currentStateData,
            float deltaTime)
        {
            Vector2 moveDirection = new Vector2(0, 0);
            
            bool w = input.Keyinputs[0];
            bool a = input.Keyinputs[1];
            bool s = input.Keyinputs[2];
            bool d = input.Keyinputs[3];
            bool space = input.Keyinputs[4];
            
            if (w || a || s || d || space)
            {
                if (w) moveDirection.Y = 1;
                if (s) moveDirection.Y = -1;

                if (a) moveDirection.X = -1;
                if (d) moveDirection.X = 1;

                moveDirection = moveDirection * deltaTime;
            }

            return moveDirection;
        }
    }
}