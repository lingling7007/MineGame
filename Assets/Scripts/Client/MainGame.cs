using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGame
{
    public class MainGame : MonoBehaviour
    {

        private static MainGame instance = null;
        public static MainGame Instance { get { return instance; } }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                gameObject.AddComponent<InGameLog>();
                DontDestroyOnLoad(this);

            }

        }
        private void Start()
        {

            RegisterInterfaceManager.RegisteIInit(QLog.Instance);

            RegisterInterfaceManager.RegisteIInit(GameStateManager.Instance);


        }
        private void Update()
        {
            RegisterInterfaceManager.Update(Time.deltaTime);
        }
        private void LateUpdate()
        {
            RegisterInterfaceManager.LateUpdate(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            RegisterInterfaceManager.FixUpdate();
        }

        private void OnDestroy()
        {
            RegisterInterfaceManager.OnRelease();
        }

    }


}
