
using UnityEngine;

namespace Assets.ManusVR.Scripts.Factory
{
    public enum HandType
    {
        Normal,
        Physics
    }

    public static class HandFactory
    {
        public static Finger GetFinger(GameObject parent, FingerIndex index, HandType handType, Hand hand, device_type_t deviceType,
            GameObject phalange0, GameObject phalange1, GameObject phalange2)
        {
            Finger finger;
            switch (handType)
            {
                case HandType.Normal:
                    finger = parent.AddComponent<RegularFinger>();
                    break;
                default:
                    finger = parent.AddComponent<RegularFinger>();
                    break;
            }
            finger.Index = index;
            finger.HandType = handType;
            finger.DeviceType = deviceType;
            finger.Phalanges[1] = phalange0;
            finger.Phalanges[2] = phalange1;
            finger.Phalanges[3] = phalange2;
            finger.Hand = hand;
            return finger;
        }

        public static Wrist GetWristController(GameObject parent, HandType type, device_type_t deviceType, Hand hand)
        {
            Wrist wrist;
            switch (type)
            {
                case HandType.Normal:
                    wrist = parent.AddComponent<RegularWrist>();
                    break;
                default:
                    wrist = parent.AddComponent<RegularWrist>();
                    break;
            }
            wrist.DeviceType = deviceType;
            wrist.Hand = hand;
            return wrist;
        }


        public static Hand GetHand(GameObject parent, HandType type, HandData handData, HandManager manager, device_type_t deviceType)
        {
            // Disable object so settings can be set before Awake() is called
            parent.SetActive(false);
            Hand hand;

            switch (type)
            {
                case HandType.Normal:
                    hand = parent.AddComponent<RegularHand>();
                    break;
                default:
                    hand = parent.AddComponent<RegularHand>();
                    break;
            }
            hand.HandType = type;
            hand.DeviceType = deviceType;
            hand.HandData = handData;
            hand.HandManager = manager;

            parent.SetActive(true);
            return hand; 
        }
    }
}