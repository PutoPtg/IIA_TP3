using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Gravar estatisticas
// Acrescentar mais dados como: desvio padrão e o resto que é pedido no enunciado
// Está a ser colocado o melhor e a média
public class StatisticsLogger {
	
	public Dictionary<int,float> bestFitness;
	public Dictionary<int,float> meanFitness;
    public Dictionary<int, float> piorFitness;
    public Dictionary<int, float> desvioFitness;

    // Variáveis para estatisticas
    int conta = 0;
    float variancia = 0;
    float desvioPadrao = 0;

    private string filename;
	private StreamWriter logger;


	public StatisticsLogger(string name) {
		filename = name;
		bestFitness = new Dictionary<int,float> ();
		meanFitness = new Dictionary<int,float> ();
        piorFitness = new Dictionary<int, float>();
        desvioFitness = new Dictionary<int, float>();
	}

	//saves fitness info and writes to console
	public void PostGenLog(List<Individual> pop, int currentGen) {
        conta = 0;
        variancia = 0;
        float media;

		pop.Sort((x, y) => x.fitness.CompareTo(y.fitness));
	
		bestFitness.Add (currentGen, pop[0].fitness);
		meanFitness.Add (currentGen, 0f);

		foreach (Individual ind in pop) {
            conta++;
			meanFitness[currentGen]+=ind.fitness;
            
		}

        media = meanFitness [currentGen] /= pop.Count;

        foreach (Individual ind in pop)
        {
            variancia += Mathf.Pow(ind.fitness - media, 2);
        }
        variancia = variancia / (conta - 1);
        desvioPadrao = Mathf.Sqrt(variancia);
        desvioFitness[currentGen] = desvioPadrao;

        piorFitness.Add(currentGen, pop[conta-1].fitness);

		Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen] + "\tpior: " + piorFitness[currentGen] +  "\tdesvio: " + desvioFitness[currentGen]+"\n");
	}


	//writes to file
	public void finalLog() {
		logger = File.CreateText (filename);

		//writes with the following format: generation, bestfitness, meanfitness
		for (int i=0; i<bestFitness.Count; i++) {
			logger.WriteLine(i+" Melhor: "+bestFitness[i]+" || Média: "+meanFitness[i] + " || Pior: " + piorFitness[i] + " || Desvio: " + desvioFitness[i]);
		}

        logger.Close ();
	}
}
