using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public GameObject plantillaMensaje;
    public GameObject messagesContainer;

    public Sprite messageLeft;
    public Sprite messageRight;

    public List<GameObject> mensajes;

    public TMP_InputField input;
    FirebaseFirestore instance;
    Usuario usuario;
    public void OnMessageSend()
    {
        if(input.text == "")
        {
            return;
        }
        DocumentReference docRef = instance.Collection("mensajes").Document();
        Mensaje mensaje = new Mensaje(usuario.Uid, usuario.Nombre, input.text);
        docRef.SetAsync(mensaje.GetDictionary());
        input.text = "";
    }

    public void iniciateMessageService(Usuario usr) {
        instance = FirebaseFirestore.GetInstance(FirebaseConection.App);
        usuario = usr;
        Query query = instance.Collection("mensajes");
        query = query.OrderBy("fecha");
        ListenerRegistration listener = query.Listen(snapshot =>
        {
        if (snapshot.Count > 0)
        {
            Dictionary<string, DocumentSnapshot> d = snapshot.ToDictionary(p => p.Id);
            foreach (KeyValuePair<string, DocumentSnapshot> item in d)
            {
                Mensaje nuevoMensaje = new Mensaje(item.Value.ToDictionary());

                bool esta = false;
                for (int i = 0; i < mensajes.Count; i++)
                {
                    if (mensajes[i].name == item.Value.Id)
                    {
                        esta = true;
                    }
                }
                if (!esta)
                {
                    GameObject go = Instantiate(plantillaMensaje, messagesContainer.transform);
                    go.GetComponent<MessageView>().setMessage(nuevoMensaje.MensajeP, nuevoMensaje.Nombre);
                    if (nuevoMensaje.SenderId == usr.Uid)
                    {
                        go.GetComponent<Image>().sprite = messageRight;
                    } else {
                        go.GetComponent<Image>().sprite = messageLeft;
                    }
                    go.name = item.Value.Id;
                    mensajes.Add(go);
                }
            }
        }

        if (mensajes.Count > 8)
        {
                messagesContainer.GetComponent<RectTransform>().offsetMax = new Vector2(messagesContainer.GetComponent<RectTransform>().offsetMax.x, (mensajes.Count - 8) * 150);
            }
        });
    }
}


public class Mensaje
{
    string senderID;
    string nombre;
    string mensaje;
    long fecha;

    public Mensaje(string uid, string nombre, string mensaje)
    {
        this.senderID = uid;
        this.nombre = nombre;
        this.mensaje = mensaje;
        this.fecha = DateTime.Now.Ticks;
    }

    public string SenderId { get { return senderID; } }
    public string MensajeP { get { return mensaje; } }
    public string Nombre { get { return nombre; } }


    public Dictionary<string, object> GetDictionary()
    {
        Dictionary<string, object> d = new Dictionary<string, object>
        {
            { "senderID", this.senderID },
            { "nombre", this.nombre },
            { "mensaje", this.mensaje },
            { "fecha", this.fecha },
        };
        return d;
    }

    public Mensaje(Dictionary<string, object> diccionario)
    {
        this.senderID = diccionario["senderID"].ToString();
        this.nombre = diccionario["nombre"].ToString();
        this.mensaje = diccionario["mensaje"].ToString();
        this.fecha = long.Parse(diccionario["fecha"].ToString());
    }

    public override string ToString()
    {
        return senderID + " - " + nombre + " - " + mensaje;
    }
}