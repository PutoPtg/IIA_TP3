using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelecaoTorneio : SelectionMethod
{
    int tam;
    public SelecaoTorneio(int tamTorneio) : base()
    {
        tam = tamTorneio;
    }

    // gera n individuos para população
    // recebe população e numero de individuos que quero selecionar
    public override List<Individual> selectIndividuals(List<Individual> oldpop, int num)
    {
        return torneioSelection(oldpop, num);
    }


    List<Individual> torneioSelection(List<Individual> oldpop, int num)
    {
        List<Individual> selectedInds = new List<Individual>();
        List<Individual> listaTorneio = new List<Individual>();
        int popsize = oldpop.Count;
        

        // Repete o numero de individuos que há na população
        for (int i = 0; i < num; i++)
        {
            // Limpa a lista do torneio, pois cada um é independente
            listaTorneio.Clear();
            for(int j=0; j<tam; j++)
            {
                // Escolhe individuo aleatório
                Individual ind = oldpop[Random.Range(0, popsize)];
                listaTorneio.Add(ind);
            }

            // Compara os individuos selecionados em relação à sua aptidão
            IComparer<Individual> aptidao = new Best();

            // Ordenação da aptidão por ordem crescente
            listaTorneio.Sort(aptidao);

            // Vai buscar melhor individuo

            Individual bestIndividuo = listaTorneio[0];
            
            selectedInds.Add(bestIndividuo.Clone()); //we return copys of the selected individuals*/
        }

        return selectedInds;
    }

}

public class Best : IComparer<Individual>
{
    public int Compare(Individual a, Individual b)
    {
        return (a.fitness).CompareTo(b.fitness);
    }
}
