using UnityEngine;
using System.Collections;

public class RobotComponentBean {
    public class LocalPlacement
        {
            private float positionX;
            private float positionY;
            private float positionZ;

            private float rotationX;
            private float rotationY;
            private float rotationZ;
        }

        public int id { get; set; }
        public string name { get; set; }
        public LocalPlacement localPlacement { get; set; }

        //CONSTRUCTOR
        public RobotComponentBean(int id, string name, LocalPlacement localPlacement)
        {
            this.localPlacement = localPlacement;
            this.id = id;
            this.name = name;
        }

        public RobotComponentBean(int id, string name)
        {
            this.localPlacement = null;
            this.name = name;
            this.id = id;
        }
}
