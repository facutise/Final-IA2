using System.Collections.Generic;
using System.Linq;

public class GoapState
{
    public WorldState worldState;


    public GoapAction generatingAction = null;
    public int step = 0;

    #region CONSTRUCTOR
    public GoapState(GoapAction gen = null)
    {
        generatingAction = gen;
        worldState = new WorldState()
        {
            /*values = new Dictionary<string, bool>()*/ // Muy importane inicializarlo en este caso
        };
    }

    public GoapState(GoapState source, GoapAction gen = null)
    {
        worldState = source.worldState.Clone();
        generatingAction = gen;
    }
    #endregion

   
}


//Nuestro estado de mundo
//Aca hay una mezcla de lo  anterior con lo nuevo, no necesariamente tiene que haber un diccionario aca adentro
public struct WorldState
{
   
    public bool IHaveChest;
    public int GoldQuantity;
    public string TengoArma;
    public float Fervor;
    public bool Password;


   // public Dictionary<string, bool> values; //Eliminar y utilizar todas las variables como playerHP

    //MUY IMPORTANTE TENER UN CLONE PARA NO TENER REFENCIAS A LO VIEJO
    public WorldState Clone()
    {
        return new WorldState()
        {
           
            IHaveChest = this.IHaveChest,
            GoldQuantity = this.GoldQuantity,
            TengoArma=this.TengoArma,
            Fervor = this.Fervor,
            Password = this.Password,

            
        };
    }
}
