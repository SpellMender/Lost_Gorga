﻿using Lost_Gorga.Old.Alt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Lost_Gorga.Old
{
    public class LoadingZone : MonoBehaviour
    {
        [SerializeField]
        bool usePlayerYpos;

        [SerializeField]
        int sceneToLoad, conditionalSceneIf;

        [SerializeField]
        bool coinHeld, gemHeld, heartHeld;

        bool conditionIsMet;

        [SerializeField]
        List<Vector2> positions = new List<Vector2>();

        [SerializeField]
        int positionIndex;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                float yPos = positions[positionIndex].y;
                if (usePlayerYpos) { yPos = AltPlayer.instance.transform.position.y; }

                AltPlayer.instance.afterFirstFrame = false;
                //AltPlayer.instance.StartWaitFrames();

                //AltPlayer.instance.firstFrameMoveInput = Input.GetAxisRaw("Horizontal");
                AltPlayer.instance.transform.position = new Vector2(positions[positionIndex].x, yPos);

                if ((coinHeld && Game.Data.Coin) || (gemHeld && Game.Data.Gem) || (heartHeld && Game.Data.Heart))
                {
                    conditionIsMet = true;
                }

                if (conditionIsMet) SceneManager.LoadScene(conditionalSceneIf);
                else SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
