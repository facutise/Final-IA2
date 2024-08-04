using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

public class Planner : MonoBehaviour
{
    /*
    public ParticleSystem particlePrefab;
    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();
    public MeshRenderer _renderer;
    public Material _runMaterial;
    public Transform chestTransform;
    public Transform enemyTransform;
    public Transform knifeTransform;
    public GameObject playerKnife;
    public GameObject equippedKnife;
    public TextMeshPro conversationText;

    private bool isMoving = false;
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();

    public float contador;
    public bool StartSequenceBool;
    private void MoveTowardsTarget(Transform target)
    {
        float step = 5 * Time.deltaTime; // Velocidad del movimiento
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            isMoving = false;
        }
    }

    void Update()
    {
       
        if (StartSequenceBool == true) return;

        contador += Time.deltaTime;
        if (contador >= 4)
        {
            StartSequenceBool = true;

            StartCoroutine(ProcessActions());

        }
    }
   
    public void EnqueHeal()
    {
        EnqueueAction(Heal());
    }
    public IEnumerator Heal()
    {
        var particles = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
        particles.Play();
        yield return null;
    }
    public void GoToChest()
    {
        EnqueueAction(GoToPosition(chestTransform, 10f));
    }

    public void GoToEnemy()
    {
        EnqueueAction(GoToPosition(enemyTransform, 0f, () => Destroy(enemyTransform.gameObject)));
    }

    public void GoToKnife()
    {
        EnqueueAction(GoToPosition(knifeTransform, 0f, () =>
        {
            knifeTransform.gameObject.SetActive(false);
            equippedKnife.SetActive(true);
        }));
    }

    public void StartConversation()
    {
        EnqueueAction(Conversation());
    }

    private IEnumerator GoToPosition(Transform target, float waitTime, System.Action onArrival = null)
    {
        isMoving = true;
        yield return new WaitUntil(() => !isMoving);

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        onArrival?.Invoke();
    }

    private IEnumerator Conversation()
    {
        string[] conversationLines = new string[]
        {
            "Hola, ¿cómo estás?",
            "Estoy bien, ¿y tú?",
            "Muy bien, gracias.",
            "Qué bueno, ¿qué has hecho últimamente?",
            "He estado trabajando en un proyecto de Unity.",
            "¡Genial! ¿De qué trata?",
            "Es un juego de aventuras.",
            "Suena interesante, ¡buena suerte!"
        };

        for (int i = 0; i < conversationLines.Length; i++)
        {
            conversationText.text = conversationLines[i];
            yield return new WaitForSeconds(1f);
        }

        conversationText.text = "";
    }

    private void EnqueueAction(IEnumerator action)
    {
        actionQueue.Enqueue(action);
        if (StartSequenceBool == true)
        {
            StartCoroutine(ProcessActions());
        }
    }

    private IEnumerator ProcessActions()
    {
        while (actionQueue.Count > 0)
        {
            yield return StartCoroutine(actionQueue.Dequeue());
        }
    }*/
    public ParticleSystem particlePrefab;
    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();
    public MeshRenderer _renderer;
    public Material _runMaterial;
    public Transform chestTransform;
    public Transform enemyTransform;
    public Transform knifeTransform;
    public GameObject equippedKnife;
    public TextMeshPro conversationText;

    private bool isMoving = false;
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();

    public float contador;
    public bool StartSequenceBool;
    public Transform currentTarget;
    private void MoveTowardsTarget(Transform target)
    {
        float step = 5 * Time.deltaTime; // Velocidad del movimiento
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            isMoving = false;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget(currentTarget); // Asume que siempre va al cofre cuando está en movimiento
        }

        if (StartSequenceBool == true) return;

        contador += Time.deltaTime;
        if (contador >= 4 /*&& actionQueue.Count<= 5*/)
        {
            StartSequenceBool = true;
            StartCoroutine(ProcessActions());
            
        }
    }

    public void EnqueHeal()
    {
        EnqueueAction(Heal());
    }

    public IEnumerator Heal()
    {
        var particles = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
        particles.Play();
        yield return null;
    }

    public void GoToChest()
    {
        EnqueueAction(GoToPosition(chestTransform, 10f));
    }

    public void GoToEnemy()
    {
        EnqueueAction(GoToPosition(enemyTransform, 0f, () => Destroy(enemyTransform.gameObject)));
    }

    public void GoToKnife()
    {
        EnqueueAction(GoToPosition(knifeTransform, 0f, () =>
        {
            knifeTransform.gameObject.SetActive(false);
            equippedKnife.SetActive(true);
        }));
    }

    public void StartConversation()
    {
        EnqueueAction(Conversation());
    }

    private IEnumerator GoToPosition(Transform target, float waitTime, Action onArrival = null)
    {
        /*
        isMoving = true;
        Debug.Log("x");
        float step = 5 * Time.deltaTime; // Velocidad del movimiento
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            isMoving = false;
        }

        yield return new WaitUntil(() => !isMoving); // Espera hasta que el movimiento termine

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime); // Espera adicional si es necesario
        }

        onArrival?.Invoke(); // Ejecuta cualquier acción adicional al llegar
        */
        currentTarget = target;
        isMoving = true;
        yield return new WaitUntil(() => !isMoving);

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        onArrival?.Invoke();
    }
    private IEnumerator Conversation()
    {
        string[] conversationLines = new string[]
        {
            "Hola, ¿cómo estás?",
            "Estoy bien, ¿y tú?",
            "Muy bien, gracias.",
            "Qué bueno, ¿qué has hecho últimamente?",
            "He estado trabajando en un proyecto de Unity.",
            "¡Genial! ¿De qué trata?",
            "Es un juego de aventuras.",
            "Suena interesante, ¡buena suerte!"
        };

        for (int i = 0; i < conversationLines.Length; i++)
        {
            conversationText.text = conversationLines[i];
            yield return new WaitForSeconds(1f);
        }

        conversationText.text = "";
    }

    private void EnqueueAction(IEnumerator action)
    {
        actionQueue.Enqueue(action);
        if (StartSequenceBool == true && actionQueue.Count == 1)
        {
            StartCoroutine(ProcessActions());
        }
    }

    private IEnumerator ProcessActions()
    {
        while (actionQueue.Count > 0)
        {
            yield return StartCoroutine(actionQueue.Dequeue());
        }
    }
    private void Start()
    {
        StartCoroutine(Plan());
    }

    private void Check(Dictionary<string, bool> state, ItemType type)
    {
        var items = Navigation.instance.AllItems();
        var inventories = Navigation.instance.AllInventories();
        var floorItems = items.Except(inventories);
        var item = floorItems.FirstOrDefault(x => x.type == type);
        var here = transform.position;
        state["accessible" + type.ToString()] = item != null && Navigation.instance.Reachable(here, item.transform.position, _debugRayList);

        var inv = inventories.Any(x => x.type == type);
        state["otherHas" + type.ToString()] = inv;

        state["dead" + type.ToString()] = false;
    }

    private IEnumerator Plan()
    {
        yield return new WaitForSeconds(0.2f);

        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;
        var floorItems = nav.AllItems();
        var inventory = nav.AllInventories();
        var everything = nav.AllItems().Union(nav.AllInventories());

        Check(observedState, ItemType.Key);
        Check(observedState, ItemType.Entity);
        Check(observedState, ItemType.Mace);
        Check(observedState, ItemType.PastaFrola);
        Check(observedState, ItemType.Door);
        Check(observedState, ItemType.Pocion);

        GoapState initial = new GoapState
        {
            worldState = new WorldState
            {
                playerHP = 88,
                cercaDeItem = true,
                espacioDeInventario = 1,
                energia = 15,
                enRangoDeAtaque = false,
                tieneArmaEquipada = "none",
                tenerPocionDeCuracion = false,
                enCombate = false,
                enUbicacionDeLaMision = false,
                misionCompletada = false
            }
        };

        var actions = CreatePossibleActionsList();

        GoapState goal = new GoapState
        {
            worldState = new WorldState
            {
                misionCompletada = true
            }
        };

        Func<GoapState, float> heuristic = (curr) =>
        {
            int count = 0;
            if (!curr.worldState.misionCompletada)
            {
                count++;
            }
            return count;
        };

        Func<GoapState, bool> objective = (curr) =>
        {
            return curr.worldState.misionCompletada;
        };
        /*

        var plan = Goap.Execute(initial, null, objective, heuristic, actions);

        if (plan == null)
            Debug.Log("Couldn't plan");
        else
        {
            var actDict = new Dictionary<string, ActionEntity>
            {
                { "Recoger Item/ cuchillo", ActionEntity.PickUp },
                { "Atacar", ActionEntity.Kill },
                { "Curarse", ActionEntity.Success },
                { "Correr", ActionEntity.Success },
                { "Lootear", ActionEntity.PickUp }
            };

            GetComponent<Guy>().ExecutePlan(
                plan
                .Select(a =>
                {
                    Item i2 = everything.FirstOrDefault(i => i.type == a.item);
                    if (actDict.ContainsKey(a.Name) && i2 != null)
                    {
                        return Tuple.Create(actDict[a.Name], i2);
                    }
                    else
                    {
                        return null;
                    }
                }).Where(a => a != null).ToList()
            );
        }*/
        var plan = Goap.Execute(initial, null, objective, heuristic, actions);

        if (plan == null)
            Debug.Log("Couldn't plan");
        else
        {
            Debug.Log("Plan generated successfully");
            foreach (var action in plan)
            {
                Debug.Log("Action: " + action.Name);
                if (action.Name == "Recoger Item/ cuchillo") GoToKnife();
                else if (action.Name =="Atacar")GoToEnemy();
                else if(action.Name =="Curarse")EnqueHeal();
                else if(action.Name=="Cofre")GoToChest();
                else if( action.Name =="Conversar")StartConversation();

            }

            var actDict = new Dictionary<string, ActionEntity>
    {
        { "Recoger Item/ cuchillo", ActionEntity.Open },
        { "Atacar", ActionEntity.Kill },
        { "Curarse", ActionEntity.Success },
        { "Cofre", ActionEntity.MoveToPastaFrola },
        { "Conversar", ActionEntity.PickUp }
    };

            GetComponent<Guy>().ExecutePlan(
                plan.Select(a =>
                {
                    Item i2 = everything.FirstOrDefault(i => i.type == a.item);
                    if (actDict.ContainsKey(a.Name) && i2 != null)
                    {
                        return Tuple.Create(actDict[a.Name], i2);
                    }
                    else
                    {
                        return null;
                    }
                }).Where(a => a != null).ToList()
            );
        }
    }

    private List<GoapAction> CreatePossibleActionsList()
    {
        return new List<GoapAction>
        {
            new GoapAction("Recoger Item/ cuchillo")
                .SetCost(20f)
                .SetItem(ItemType.Key)
                .Pre(gS => gS.worldState.playerHP > 50 )
                .Effect(gS =>
                {
                    gS.worldState.espacioDeInventario += 1;
                    gS.worldState.energia -= 1;
                    gS.worldState.tieneArmaEquipada = "Cuchillo";
                    //GoToKnife();
                    Debug.Log("A");
                    return gS;
                }),
            new GoapAction("Atacar")
                .SetCost(30f)
                .SetItem(ItemType.Mace)
                .Pre(gS => /*gS.worldState.tieneArmaEquipada == "Cuchillo" && gS.worldState.enRangoDeAtaque &&*/ gS.worldState.playerHP > 5)
                .Effect(gS =>
                {
                    gS.worldState.playerHP -= 4;
                    gS.worldState.energia -= 5;
                    //GoToEnemy();
                    Debug.Log("b");
                    return gS;
                }),
            new GoapAction("Curarse")
                .SetCost(3f)
                .Pre(gS => /*!gS.worldState.enCombate && gS.worldState.tenerPocionDeCuracion && */gS.worldState.playerHP < 10)
                .Effect(gS =>
                {
                    gS.worldState.playerHP += 4;
                    gS.worldState.energia -= 2;
                    //EnqueHeal();
                    Debug.Log("c");
                    return gS;
                }),
            new GoapAction("Cofre")
                .SetCost(4f)
                .Pre(gS => gS.worldState.enUbicacionDeLaMision/* && !gS.worldState.enCombate && gS.worldState.energia >= 15*/)
                .Effect(gS =>
                {
                    gS.worldState.enUbicacionDeLaMision = true;
                    gS.worldState.energia -= 15;
                    gS.worldState.misionCompletada = true;
                    //StartConversation();
                    //GoToChest();
                    Debug.Log("d");
                    //_renderer.material= _runMaterial;
                    return gS;

                }),
            new GoapAction("Conversar")
                .SetCost(1f)
                //.SetItem(ItemType.Pocion) WTF??!!!!
                .Pre(gS => /*!gS.worldState.tenerPocionDeCuracion && gS.worldState.espacioDeInventario >= 5 && */gS.worldState.energia >= 2)
                .Effect(gS =>
                {
                    gS.worldState.espacioDeInventario -= 5;
                    gS.worldState.energia -= 2;
                    gS.worldState.tenerPocionDeCuracion = true;
                    gS.worldState.enUbicacionDeLaMision=true;

                    //StartConversation();
                    Debug.Log("e");

                    return gS;
                })
        };
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var t in _debugRayList)
        {
            Gizmos.DrawRay(t.Item1, (t.Item2 - t.Item1).normalized);
            Gizmos.DrawCube(t.Item2 + Vector3.up, Vector3.one * 0.2f);
        }
    }

}
/*
public class LeaderStateMachine : MonoBehaviour
{

    #region Variables

    [SerializeField] ViewDetection viewDetection;
    [SerializeField] Character character;
    [SerializeField] PhysicalNodeGrid physicalNodeGrid;
    [SerializeField] Pathfinder pathfinder;
    [SerializeField] List<Transform> enemyList;
   
    [SerializeField] Renderer render;
    [SerializeField] Material leftMaterial;
    [SerializeField] Material rightMaterial;
    [SerializeField] private float fleeDuration = 3f;
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float attackDistance = 2.0f;
    [SerializeField] private float bonusSpeed = 1.5f;
    [SerializeField] private int initialHealth = 100;
    [SerializeField] private int healthToFlee = 30;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isLeftLider;

    MaterialPropertyBlock block;


    private Node lastNode;
    private Node clickNode;
  


    public State currentState;

    public enum State
    {
        LiderMove,
       
    }

    #endregion

    private void Awake()
    {
        block = new MaterialPropertyBlock { };
      

       
    }

    #region Manejo de cambio de estados

    private void Update()
    {
       
        LiderMove();
    }
    
    #endregion

    #region Funciones Principales

    //LIDERMOVE: Funcion que reconoce el nodo mas cercano a la posicion del mouse, el objeto asociado a este objeto se dirije hacia este objeto.
    // Es importante saber si isLeftLider es true, porque si lo es, atiende al click izquierdo. Si es falso, responde al click derecho
    private void LiderMove()
    {

        block.SetColor("_Color", Color.black);
        render.SetPropertyBlock(block);

        if (Input.GetMouseButtonDown(0) && !isMoving && isLeftLider == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                clickNode = physicalNodeGrid.GetClosest(hit.point);

                if (clickNode != null)
                {
                    //clickNode.ChangeNodeMaterialTemporary(leftMaterial, 1.5f);
                    PathMove();
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) && !isMoving && isLeftLider == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                clickNode = physicalNodeGrid.GetClosest(hit.point);

                if (clickNode != null)
                {
                   // clickNode.ChangeNodeMaterialTemporary(rightMaterial, 1.5f);
                    PathMove();
                }
            }
        }
    }

    // Es la misma funcion que use en el parcial 2.
    // Lo que hace es llamar al pathfinder y mover al objeto por pathfinder. Se verifica que el camino sea nulo, se asigna como current el nodo mas cercano al lider
    // y como target el nodo clickeado. Una vez se tienen estos nodos se crea un path por el pathfinder y hace que el lider lo recorra
    private void PathMove()
    {
        if (pathfinder.current == null)
        {
            lastNode = physicalNodeGrid.GetClosest(clickNode.WorldPosition);
            pathfinder.current = physicalNodeGrid.GetClosest(transform.position);
        }


        pathfinder.target = lastNode;
        pathfinder.path = pathfinder.CallPathfind(lastNode);

        if (pathfinder.path.Count > 0)
        {
            pathfinder.current = pathfinder.path[0];
        }
        else
        {
            return;
        }

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastNode);
    }


    // Esta funcion tambien es sacada del parcial 2
    // Lo que hace este script es encargarse de que se vaya moviendo a traves de los nodos que se asignaron en el path hecho por el pathfinder
    // Una vez se completa el camino se borra el path y se borra la referencia del current y del target para que no haya problemas al generar un nuevo camino
    private IEnumerator FollowPathAndCheckForPlayer()
    {
        isMoving = true;
        int targetIndex = 1;

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position).normalized;
                character.velocity = dir * movementSpeed;

                yield return null;
            }

            character.velocity = Vector3.zero;

            if (targetIndex < pathfinder.path.Count - 1)
            {
                targetIndex++;
                pathfinder.current = pathfinder.path[targetIndex - 1];
            }
            else
            {
                pathfinder.path.Clear();
                pathfinder.current = null;
                pathfinder.target = null;
                lastNode = null;
                isMoving = false;
                yield break;
            }
        }

        character.velocity = Vector3.zero;
        isMoving = false;
    }

  

    // Este script tomo base en el avoid obstacle del primer parcial
    // Se declara un radio para esquivar y una fuerza de esquive (los dejo en 5 porque si no se queda orbitando) y se genera un vector que sera la direccion de esquive
    // Se genera rayos para simular un circulo, esto se hace ya que usar un sphere collider no lo toma, se generan 8 para cubrir varios angulos
    // Cada rayo genera un raycast para distinguir los obstaculos, si se identifica un "Wall" se aplica la fuerza de avoid junto con la direccion calculada para el esquive
    // La fuerza se suma a la direccion
    // dicha fuerza se aplica a la velocidad del script de character.

    // Hay un tema que no pude solucionar que es que a veces el avoid aumenta brevemente la velocidad del character.
    private void AvoidObstacles()
    {
        float radius = 5f;
        float force = 5f;

        Vector3 avoidanceDirection = Vector3.zero;

        int rayCount = 8;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rayDirection = rotation * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    float avoidForce = Mathf.Clamp01((radius - hit.distance) / radius) * force;

                    Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up);

                    character.velocity += avoidDirection * avoidForce;
                    Debug.DrawRay(hit.point, avoidDirection, Color.magenta);
                }
            }
        }
    }

    #endregion

}*/










/*
    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();

    private void Start()
    {
        StartCoroutine(Plan());
    }

    private void Check(Dictionary<string, bool> state, ItemType type)
    {

        var items = Navigation.instance.AllItems();
        var inventories = Navigation.instance.AllInventories();
        var floorItems = items.Except(inventories);//devuelve una coleccion como la primera pero removiendo los que estan en la segunda
        var item = floorItems.FirstOrDefault(x => x.type == type);
        var here = transform.position;
        state["accessible" + type.ToString()] = item != null && Navigation.instance.Reachable(here, item.transform.position, _debugRayList);

        var inv = inventories.Any(x => x.type == type);
        state["otherHas" + type.ToString()] = inv;

        state["dead" + type.ToString()] = false;
    }

    //No es necesario que sea una corrutina excepto que se calcule GOAP con timeslicing.
    private IEnumerator Plan()
    {
        yield return new WaitForSeconds(0.2f);


        #region Si no se usan objetos modulares, se puede eliminar
        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;//Consigo los items
        var floorItems = nav.AllItems();
        var inventory = nav.AllInventories();
        var everything = nav.AllItems().Union(nav.AllInventories());// .Union() une 2 colecciones sin agregar duplicados(eso incluye duplicados en la misma coleccion)

        //Chequeo los booleanos para cada Item, generando mi modelo de mundo (mi diccionario de bools) en ObservedState
        //Facu: Estos Check buscan los objetos en el mundo y se lo "asignan"/"conecta" al nodo más cercano
        Check(observedState, ItemType.Key);
        Check(observedState, ItemType.Entity);
        Check(observedState, ItemType.Mace);
        Check(observedState, ItemType.PastaFrola);
        Check(observedState, ItemType.Door);
        Check(observedState, ItemType.Pocion);
        #endregion

        GoapState initial = new GoapState(); //Crear GoapState
        initial.worldState = new WorldState()
        {

            playerHP = 88,
            cercaDeItem = true,
            espacioDeInventario = 1,
            energia = 15,
            enRangoDeAtaque = false,
            tieneArmaEquipada = "none",
            tenerPocionDeCuracion = false,
            enCombate = false,
            enUbicacionDeLaMision = false,
            misionCompletada = false,


        };


        //Si uso items modulares:
        //initial.worldState.values = observedState;  //le asigno los valores actuales, conseguidos antes
        //initial.worldState.values["doorOpen"] = false; //agrego el bool "doorOpen"

        //Calculo las acciones
        var actions = CreatePossibleActionsList();

        #region opcional

        #endregion

        //Es opcional, no es necesario buscar por un nodo que cumpla perfectamente con las condiciones
        GoapState goal = new GoapState();

        goal.worldState.misionCompletada= true;


        //Crear la heuristica personalizada para no calcular nodos de mas
        Func<GoapState, float> heuristic = (curr) =>
        {
            int count = 0;

            if (curr.worldState.misionCompletada =! true)// Creo, no estoy seguro
            {
                count++;
            }
            return count;
        };

        //Esto seria el reemplazo de goal, donde se pide que cumpla con las condiciones pasadas.
        Func<GoapState, bool> objective = (curr) =>
         {

             return curr.worldState.misionCompletada = true;
         };

        #region Opcional
        var actDict = new Dictionary<string, ActionEntity>() {
              { "Kill"  , ActionEntity.Kill }
            , { "Pickup", ActionEntity.PickUp }
            , { "Open"  , ActionEntity.Open }
        };
        #endregion

        var plan = Goap.Execute(initial, null, objective, heuristic, actions);

        if (plan == null)
            Debug.Log("Couldn't plan");
        else
        {
            GetComponent<Guy>().ExecutePlan(
                plan
                .Select(a =>
                {
                    Item i2 = everything.FirstOrDefault(i => i.type == a.item);
                    if (actDict.ContainsKey(a.Name) && i2 != null)
                    {
                        return Tuple.Create(actDict[a.Name], i2);
                    }
                    else
                    {
                        return null;
                    }
                }).Where(a => a != null)
                .ToList()
            );
        }
    }

    private List<GoapAction> CreatePossibleActionsList()
    {
        return new List<GoapAction>()
        {

              new GoapAction("Recoger Item/ cuchillo")
                .SetCost(2f)
                .SetItem(ItemType.Key) 

                .Pre((gS)=>
                {

                           return gS.worldState.playerHP > 50 && gS.worldState.cercaDeItem==true&&
                    gS.worldState.espacioDeInventario>0&&gS.worldState.energia>0;
                })
                //Ejemplo de setteo de Effect
                .Effect((gS) =>
                    {

                       gS.worldState.espacioDeInventario+=1; gS.worldState.energia-=1;  gS.worldState.tieneArmaEquipada=("Cuchillo");
                        return gS;
                    }
                )
                , new GoapAction("Atacar")
                .SetCost(5f)
                .SetItem (ItemType.Mace)
                .Pre((gS)=>
                {

                           return gS.worldState.tieneArmaEquipada==("Cuchillo") && gS.worldState.enRangoDeAtaque==true&&
                    gS.worldState.playerHP>5;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.playerHP-=4; gS.worldState.energia-=5;
                        return gS;
                    }
                )

                , new GoapAction("Curarse")
                .SetCost(3f)
                .SetItem (ItemType.Pocion)
                .Pre((gS)=>
                {

                           return gS.worldState.enCombate==false && gS.worldState.tenerPocionDeCuracion==true&&
                    gS.worldState.playerHP<10;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.playerHP+=4; gS.worldState.energia-=2;
                        return gS;
                    }
                )

                , new GoapAction("Correr")
                .SetCost(1f)
                .SetItem (ItemType.PastaFrola)
                .Pre((gS)=>
                {

                           return gS.worldState.enUbicacionDeLaMision==false && gS.worldState.enCombate==false&&
                    gS.worldState.energia>=15;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.enUbicacionDeLaMision=true; gS.worldState.energia-=15; gS.worldState.misionCompletada=true;
                        return gS;
                    }
                )

                , new GoapAction("Lootear")
                .SetCost(2f)
                .Pre((gS)=>
                {

                           return gS.worldState.tenerPocionDeCuracion==false && gS.worldState.espacioDeInventario>=5&&
                    gS.worldState.energia>=2;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.espacioDeInventario-=5; gS.worldState.energia-=2; ; gS.worldState.tenerPocionDeCuracion=true;
                        return gS;
                    }
                )


        };
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var t in _debugRayList)
        {
            Gizmos.DrawRay(t.Item1, (t.Item2 - t.Item1).normalized);
            Gizmos.DrawCube(t.Item2 + Vector3.up, Vector3.one * 0.2f);
        }
    }*/