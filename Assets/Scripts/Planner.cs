using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Planner : MonoBehaviour
{
   

    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();

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
            }

            var actDict = new Dictionary<string, ActionEntity>
    {
        { "Recoger Item/ cuchillo", ActionEntity.Open },
        { "Atacar", ActionEntity.Kill },
        { "Curarse", ActionEntity.Success },
        { "Correr", ActionEntity.Success },
        { "Lootear", ActionEntity.PickUp }
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
                .SetCost(2f)
                .SetItem(ItemType.Key)
                .Pre(gS => gS.worldState.playerHP > 50 )
                .Effect(gS =>
                {
                    gS.worldState.espacioDeInventario += 1;
                    gS.worldState.energia -= 1;
                    gS.worldState.tieneArmaEquipada = "Cuchillo";
                    return gS;
                }),
            new GoapAction("Atacar")
                .SetCost(5f)
                .SetItem(ItemType.Mace)
                .Pre(gS => /*gS.worldState.tieneArmaEquipada == "Cuchillo" && gS.worldState.enRangoDeAtaque &&*/ gS.worldState.playerHP > 5)
                .Effect(gS =>
                {
                    gS.worldState.playerHP -= 4;
                    gS.worldState.energia -= 5;
                    return gS;
                }),
            new GoapAction("Curarse")
                .SetCost(3f)
                .SetItem(ItemType.Pocion)
                .Pre(gS => /*!gS.worldState.enCombate && gS.worldState.tenerPocionDeCuracion && */gS.worldState.playerHP < 10)
                .Effect(gS =>
                {
                    gS.worldState.playerHP += 4;
                    gS.worldState.energia -= 2;
                    return gS;
                }),
            new GoapAction("Correr")
                .SetCost(4f)
                .SetItem(ItemType.PastaFrola)
                .Pre(gS => gS.worldState.enUbicacionDeLaMision/* && !gS.worldState.enCombate && gS.worldState.energia >= 15*/)
                .Effect(gS =>
                {
                    gS.worldState.enUbicacionDeLaMision = true;
                    gS.worldState.energia -= 15;
                    gS.worldState.misionCompletada = true;
                    return gS;
                }),
            new GoapAction("Lootear")
                .SetCost(1f)
                //.SetItem(ItemType.Pocion) WTF??!!!!
                .Pre(gS => /*!gS.worldState.tenerPocionDeCuracion && gS.worldState.espacioDeInventario >= 5 && */gS.worldState.energia >= 2)
                .Effect(gS =>
                {
                    gS.worldState.espacioDeInventario -= 5;
                    gS.worldState.energia -= 2;
                    gS.worldState.tenerPocionDeCuracion = true;
                    gS.worldState.enUbicacionDeLaMision=true;
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