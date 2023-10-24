using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject character;
    public GameObject plantillaUsuario;
    List<GameObject> usuarios;

    public GameObject panel;
    public TMP_InputField input;
    public TMP_Text text;
    public Usuario usr;
    DocumentReference docRef;

    public MessageManager messageManager;

    public GameObject messageButton;
    public Sprite perro;

    void Awake()
    {
        character.transform.position = new Vector2(Random.Range(1, 24), -20);
        usuarios = new List<GameObject>();
        FirebaseConection.ConnectFirebase();
    }

    public void Entrar()
    {
        DoSomethingFirebase();
    }

    async void DoSomethingFirebase()
    {
        if(input.text == "")
        {
            return;
        }
        panel.SetActive(false);
        FirebaseFirestore instance = FirebaseFirestore.GetInstance(FirebaseConection.App);
        docRef = instance.Collection("usuarios").Document();
        text.text = input.text;
        usr = new Usuario(docRef.Id, input.text, character.transform.position.x, -0);
        await docRef.SetAsync(usr.GetDictionary());
        messageManager.iniciateMessageService(usr);
        messageButton.SetActive(true);

        Query query = instance.Collection("usuarios");
        ListenerRegistration listener = query.Listen(snapshot =>
        {
            if(snapshot.Count > 0)
            {
               Dictionary<string, DocumentSnapshot> d = snapshot.ToDictionary(p => p.Id);
                foreach (KeyValuePair<string, DocumentSnapshot> item in d)
                {
                    Usuario nuevoUsuario = new Usuario(item.Value.ToDictionary());
                    Debug.Log(nuevoUsuario.ToString());
                    if(usr.Uid != nuevoUsuario.Uid)
                    {
                        Debug.Log(nuevoUsuario.ToString());
                        bool esta = false;
                        for (int i = 0; i < usuarios.Count; i++)
                        {
                            if (usuarios[i].name == nuevoUsuario.Uid)
                            {
                                esta = true;
                                if (nuevoUsuario.Conectado)
                                {
                                    usuarios[i].transform.position = new Vector2(nuevoUsuario.PosX, -nuevoUsuario.PosY);
                                } else
                                {
                                    usuarios.Remove(usuarios[i]);
                                    Destroy(usuarios[i]);
                                }
                            }
                        }
                        if (!esta)
                        {
                            if (!nuevoUsuario.Conectado)
                            {

                            } else
                            {
                                GameObject go = Instantiate(plantillaUsuario);
                                if(nuevoUsuario.Plataforma == "Unity")
                                {
                                    go.GetComponent<SpriteRenderer>().sprite = perro;
                                }
                                go.GetComponentInChildren<TMP_Text>().text = nuevoUsuario.Nombre;
                                go.name = nuevoUsuario.Uid;
                                go.transform.position = new Vector2(nuevoUsuario.PosX, -nuevoUsuario.PosY);
                                usuarios.Add(go);
                            }
                        }
                    }
                }
            }
        });
    }

    public void Move(int direccion)
    {
        if (character.transform.position.x <= 0 && character.transform.position.x >= 25 && character.transform.position.y <= 0 && character.transform.position.y >= 25)
        {
            return;
        }

        switch ((Direccion)direccion)
        {
            case Direccion.Arriba :
                    usr.PosY -= 0.5f;
                break;
            case Direccion.Derecha:
                    usr.PosX += 0.5f;
                break;
            case Direccion.Abajo:
                    usr.PosY += 0.5f;
                break;
            case Direccion.Izquierda:
                    usr.PosX -= 0.5f;
                break;
        }

        docRef.UpdateAsync(usr.GetDictionary());
        character.transform.position = new Vector2(usr.PosX, -usr.PosY);
    }
 
    private void OnApplicationPause(bool pause)
    {
        if (docRef == null)
        {
            return;
        }
        usr.Conectado = !pause;
        docRef.SetAsync(usr.GetDictionary());
    }

}

public enum Direccion
{
    Arriba,
    Derecha,
    Abajo,
    Izquierda
}

public class Usuario
{
    string uid;
    string nombre;
    float posX;
    float posY;
    bool conectado;
    string plataforma;

    public Usuario(string uid, string nombre, float posX, float posY)
    {
        this.uid = uid;
        this.nombre = nombre;
        this.posX = posX;
        this.posY = posY;
        this.conectado = true;
        this.plataforma = "Unity";
    }

    public float PosX { get { return posX; } set { posX = value; } }
    public float PosY { get { return posY; } set { posY = value; } }
    public string Uid { get { return uid; } }
    public bool Conectado { set { this.conectado = value; } get { return this.conectado; } }
    public string Nombre { get { return this.nombre; } }
    public string Plataforma { get { return this.plataforma; } }

    public Dictionary<string, object> GetDictionary()
    {
        Dictionary<string, object> d = new Dictionary<string, object>
        {
            { "uid", this.uid },
            { "nombre", this.nombre }, 
            { "posX", this.posX }, 
            { "posY", posY }, 
            { "conectado", this.conectado }, 
            { "plataforma", plataforma }
        };
        return d;
    }

    public Usuario(Dictionary<string, object> diccionario) {
        this.uid = diccionario["uid"].ToString();
        this.nombre = diccionario["nombre"].ToString();
        this.posX = float.Parse(diccionario["posX"].ToString());
        this.posY = float.Parse(diccionario["posY"].ToString());
        this.conectado = bool.Parse(diccionario["conectado"].ToString());
        this.plataforma = diccionario["plataforma"].ToString();
    }

    public override string ToString()
    {
        return uid + " - " + nombre;
    }
}