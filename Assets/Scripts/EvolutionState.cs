﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvolutionState : MonoBehaviour {

	public float startPointX;
	public float startPointY;
	public float endPointX;
	public float endPointY;
	public float g;
	public float startVelocity;
	public int numTrackPoints;
	private ProblemInfo info;
    public int tamTorneio;      // tamanho do torneio a definir no unity
    public int selecao;         // 0 - Random, 1- Torneio
    public int numPontosCorte;  // num de pontos de corte na recombinação
    public int individuo;       // 0 - ExampleInividual ; 1 - NovoIndividuo
    public int elitismo;        // 0 - nao, 1 - sim
    public int indElitismo;     // numero de individuos escolhidos no elitismo


	public int numGenerations;
	public int populationSize;
	public float mutationProbability;
	public float crossoverProbability;

	private List<Individual> population;
	private SelectionMethod selection;

	private int evaluatedIndividuals;
	private int currentGeneration;
	public int EvaluationsPerStep;

	private StatisticsLogger stats;
	public string statsFilename;

	private PolygonGenerator drawer;


	bool evolving;
	bool drawing;

	// Use this for initialization
	void Start () {
		info = new ProblemInfo ();
		info.startPointX = startPointX;
		info.startPointY = startPointY;
		info.endPointX = endPointX;
		info.endPointY = endPointY;
		info.g = g;
		info.startVelocity = startVelocity;
		info.numTrackPoints = numTrackPoints;

        if(selecao == 0)
        {
            selection = new RandomSelection();
        }
        if(selecao == 1)
        {
            selection = new SelecaoTorneio(tamTorneio); 
        }

        
        
      
      
		stats = new StatisticsLogger (statsFilename);

		drawer = new PolygonGenerator ();

		InitPopulation ();
		evaluatedIndividuals = 0;
		currentGeneration = 0;
		evolving = true;
		drawing = false;
	}
	

	void FixedUpdate () {
		if (evolving) {
			EvolStep ();
		} else if(drawing) {
			population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
			drawer.drawCurve(population[0].trackPoints,info);
			drawing=false;
		}
	}

	void EvolStep() {
		//do for a given number of generations
		if (currentGeneration < numGenerations) {
			
			//if there are individuals to evaluate on the current generation
			int evalsThisStep = EvaluationsPerStep < (populationSize - evaluatedIndividuals) ? EvaluationsPerStep : (populationSize - evaluatedIndividuals);
			for (int ind = evaluatedIndividuals; ind<evaluatedIndividuals+evalsThisStep; ind++) {
				population[ind].evaluate();
			}
			evaluatedIndividuals += evalsThisStep;
			
			//if all individuals have been evaluated on the current generation, breed a new population
			if(evaluatedIndividuals==populationSize) {
				stats.PostGenLog(population,currentGeneration);

             
                population = BreedPopulation();
        
				
				evaluatedIndividuals=0;
				currentGeneration++;
			}
			
		} else {
			stats.finalLog();
			evolving=false;
			drawing = true;
			print ("evolution stopped");
		}
		
		

	
	}


	void InitPopulation () {
		population = new List<Individual>();
		while (population.Count<populationSize) {
            if (individuo == 0)
            {
                ExampleIndividual newind = new ExampleIndividual(info);
                newind.Initialize();
                population.Add(newind);
            }
            if (individuo == 1)
            {
                NovoIndividuo newind = new NovoIndividuo(info, numPontosCorte);
                newind.Initialize();
                population.Add(newind);
            }
		}
	}


	List<Individual> BreedPopulation() {
        List<Individual> newpop = new List<Individual>();
        // Sem elitismo
        if (elitismo == 0)
        {
            //breed individuals and place them on new population. We'll apply crossover and mutation later 
            while (newpop.Count < populationSize)
            {
                List<Individual> selectedInds = selection.selectIndividuals(population, 2); //we should propably always select pairs of individuals
                for (int i = 0; i < selectedInds.Count; i++)
                {
                    if (newpop.Count < populationSize)
                    {
                        newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
                    }
                    else { //discard any excess individuals
                        selectedInds.RemoveAt(i);
                    }
                }

                //apply crossover between pairs of individuals and mutation to each one
                while (selectedInds.Count > 1)
                {
                    selectedInds[0].Crossover(selectedInds[1], crossoverProbability);
                    selectedInds[0].Mutate(mutationProbability);
                    selectedInds[1].Mutate(mutationProbability);
                    selectedInds.RemoveRange(0, 2);
                }
                if (selectedInds.Count == 1)
                {
                    selectedInds[0].Mutate(mutationProbability);
                    selectedInds.RemoveAt(0);
                }
            }
        }
        // Com elitismo
        else
        {
            List<Individual> elite = new List<Individual>();
            //breed individuals and place them on new population. We'll apply crossover and mutation later 
            while (newpop.Count < populationSize)
            {
                List<Individual> selectedInds = selection.selectIndividuals(population, 2); //we should propably always select pairs of individuals
                elite.Add(selectedInds[0]);
                elite.Add(selectedInds[1]);

                //newpop = selectedInds.GetRange(0, indElitismo);

                for (int i = 0; i < selectedInds.Count; i++)
                {
                    if (newpop.Count < populationSize)
                    {
                        newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
                    }
                    else { //discard any excess individuals
                        selectedInds.RemoveAt(i);
                    }
                }

                //apply crossover between pairs of individuals and mutation to each one
                while (selectedInds.Count > 1)
                {
                    selectedInds[0].Crossover(selectedInds[1], crossoverProbability);
                    selectedInds[0].Mutate(mutationProbability);
                    selectedInds[1].Mutate(mutationProbability);
                    selectedInds.RemoveRange(0, 2);
                }
                if (selectedInds.Count == 1)
                {
                    selectedInds[0].Mutate(mutationProbability);
                    selectedInds.RemoveAt(0);
                }
            }
            // Compara os individuos selecionados em relação à sua aptidão
            IComparer<Individual> aptidao = new Best();

            // Ordenação da aptidão por ordem crescente
            // Ordenação da lista para depois escolher sempre os melhores dos melhores
            elite.Sort(aptidao);

            for(int i = 0; i<indElitismo; i++)
            {
                newpop[i] = elite[i];
  
            }

        }
		

		return newpop;
	}


    public class Best : IComparer<Individual>
    {
        public int Compare(Individual a, Individual b)
        {
            return (a.fitness).CompareTo(b.fitness);
        }
    }



}


