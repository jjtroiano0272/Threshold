// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DamageFlash : MonoBehaviour
// {


//     private void Awake()
//     {


//     }
//     private void Init()

//     {

//     }
//     public void CallDamageFlash()
//     {
//         _damageFlashCoroutine = StartCoroutine(DamageFlasher());
//     }

//     private IEnumerator DamageFlasher()
//     {
//         SetFlashColor();

//         float currentFlashAmount = 0f;
//         float elpasedTime = 0f;
//         while (elpasedTime < _flashTime)
//         {
//             elpasedTime += Time.deltaTime;

//             currentFlashAmount = Mathf.Lerp(1f, 0f, (elpasedTime / _flashTime));
//             SetFlashAmount(currentFlashAmount);
//             yield return null;
//         }

//     }

//     private void SetFlashColor()
//     {
//         for (int i = 0; i < _materials.Length; i++)
//         {
//             _materials[i].SetColor("_FlashColor", _flashColor);
//         }

//     }

//     private void SetFlashAmount(float amount)
//     {
//         for (int i = 0; i < _materials.Length; i++)
//         {
//             _materials[i].SetFloat("_FlashAmount", amount);

//         }
//     }

// }