
using Dawnshard.Views;
using UnityEngine;

public class MobileRaycastHandler : MonoBehaviour
{
#if UNITY_ANDROID
    private GameObject lastHitObject = null; // Tiene traccia dell'ultimo oggetto colpito

    void Update()
    {
        if (Input.touchCount > 0) // Se c'è un tocco sullo schermo
        {
            Touch touch = Input.GetTouch(0); // Prende il primo tocco

            // Raycast solo durante la fase di tocco
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position); // Crea un ray dal punto del tocco
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) // Se il ray colpisce qualcosa
                {
                    GameObject hitObject = hit.transform.gameObject;

                    // Se è un nuovo oggetto, chiama OnMouseExit sull'ultimo e OnMouseEnter sul nuovo
                    if (hitObject != lastHitObject)
                    {
                        if (lastHitObject != null)
                        {
                            // Chiama OnMouseExit sull'ultimo oggetto
                            lastHitObject.GetComponent<CardView>()?.OnMouseExit();
                        }

                        // Chiama OnMouseEnter sul nuovo oggetto
                        hitObject.GetComponent<CardView>()?.OnMouseEnter();

                        // Aggiorna l'ultimo oggetto colpito
                        lastHitObject = hitObject;
                    }
                }
                else
                {
                    // Se il raycast non colpisce nulla, chiama OnMouseExit sull'ultimo oggetto colpito
                    if (lastHitObject != null)
                    {
                        lastHitObject.GetComponent<CardView>()?.OnMouseExit();
                        lastHitObject = null; // Reset dell'ultimo oggetto
                    }
                }
            }

            // Gestisci il rilascio del tocco (facoltativo, per eventuali azioni al rilascio)
            if (touch.phase == TouchPhase.Ended)
            {
                if (lastHitObject != null)
                {
                    // Se c'era un oggetto colpito, chiama OnMouseExit al termine del tocco
                    lastHitObject.GetComponent<CardView>()?.OnMouseExit();
                    lastHitObject = null; // Reset dell'ultimo oggetto
                }
            }
        }
    }
#endif
}


