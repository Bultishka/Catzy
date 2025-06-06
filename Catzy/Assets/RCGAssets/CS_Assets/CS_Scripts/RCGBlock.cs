﻿using UnityEngine;
using RoadCrossing.Types;
using System.Reflection;

namespace RoadCrossing
{
	/// <summary>
	/// A code for a block that can be touched by the player (can be a rock, a wall, an enemy, a coin, etc.
	/// Defines characteristics of a block (what it is, what player can do to it) and its interaction with the player (nothing, kill him, etc)
	/// </summary>
	public class RCGBlock : MonoBehaviour
	{
        // The tag of the object that can touch this block
        public string touchTargetTag0 = "Player";
        public string touchTargetTag1 = "Player1";
        public string touchTargetTag2 = "Player2";


        // An array of functions that run when this block is touched by the target
        public TouchFunction[] touchFunctions;

		// Remove this object after a ceratin amount of touches
		public int removeAfterTouches = 0;
		internal bool isRemovable = false;

		// The animation that plays when this object is touched
		public AnimationClip hitAnimation;

		// The sound that plays when this object is touched
		public AudioClip soundHit;
		public string soundSourceTag = "GameController";

		// The effect that is created at the location of this object when it is destroyed
		public Transform deathEffect;

		/// <summary>
		/// define what kind of object it is (toucheble / untouchable)
		/// </summary>
		void Start()
		{
			// If removeAfterTouches is higher than 0, make this object removable after one or more touches
			if (removeAfterTouches > 0)
				isRemovable = true;
		}

		/// <summary>
		/// Is executed when this obstacle touches another object with a trigger collider
		/// </summary>
		void OnTriggerEnter(Collider other)
		{
			// Check if the object that was touched has the correct tag
			if (other.tag == touchTargetTag0 || other.tag == touchTargetTag1 || other.tag == touchTargetTag2)
			{
                // Go through the list of functions and run only those with a tag of a touched object
                foreach (var touchFunction in touchFunctions)
				{
					if(touchFunction.targetTag == other.tag || touchFunction.targetTag == "GameController")
					{
						Debug.Log(name + " вызвало " + touchFunction.functionName + " у " + other.tag + " на позиции " + other.gameObject.transform.position);
                        // Check that we have a target tag and function name before running
                        if (touchFunction.functionName != string.Empty && touchFunction.targetTag == other.tag || touchFunction.targetTag == "GameController")
                        {
                            if (touchFunction.functionName != "AttachToThis")
                            {
                                GameObject.FindGameObjectWithTag(touchFunction.targetTag).SendMessage(touchFunction.functionName, touchFunction.functionParameter);
                            }
                            else
                            {
                                GameObject.FindGameObjectWithTag(touchFunction.targetTag).SendMessage(touchFunction.functionName, transform);
                            }
                        }
                    }
				}

				// If there is an animation, play it
				if (GetComponent<Animation>() && hitAnimation)
				{
					// Stop the animation
					GetComponent<Animation>().Stop();

					// Play the animation
					GetComponent<Animation>().Play(hitAnimation.name);
				}

				// If this object is removable, count down the touches and then remove it
				if (isRemovable == true)
				{
					// Reduce the number of times this object was touched by the target
					removeAfterTouches--;

					if (removeAfterTouches <= 0)
					{
						if (deathEffect)
							Instantiate(deathEffect, transform.position, Quaternion.identity);

						Destroy(gameObject);
					}
				}

				// If there is a sound source and a sound assigned, play it
				if (soundSourceTag != string.Empty && soundHit)
					GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundHit);
			}
		}
	}
}