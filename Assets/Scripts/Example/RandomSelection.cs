using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Seleção aleatório
// tempos de criar um método de seleção por torneio que tambem deriva de SelectionMethos
public class RandomSelection : SelectionMethod {

	public RandomSelection(): base() {

	}

    // gera n individuos para população
    // recebe população e numero de individuos que quero selecionar
	public override List<Individual> selectIndividuals (List<Individual> oldpop, int num)
	{
		return randomSelection (oldpop, num);
	}


	List<Individual> randomSelection(List<Individual>oldpop, int num) {

		List<Individual> selectedInds = new List<Individual> ();
		int popsize = oldpop.Count;
		for (int i = 0; i<num; i++) {
			//make sure selected individuals are different
			Individual ind = oldpop [Random.Range (0, popsize)];
			while (selectedInds.Contains(ind)) {
				ind = oldpop [Random.Range (0, popsize)];
			}
			selectedInds.Add (ind.Clone()); //we return copys of the selected individuals
		}

		return selectedInds;
	}

}
