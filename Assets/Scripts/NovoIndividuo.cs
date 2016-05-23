using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Representação direta: genotipo = fenotipo
// trackPoints: dicionrio com todos os pontos onde passa a linha (genotipo do individuo)

public class NovoIndividuo : Individual
{

    int numPontosCorte;
    private float MinX;
    private float MaxX;
    private float MinY;
    private float MaxY;

    public NovoIndividuo(ProblemInfo info, int ptsCorte) : base(info)
    {

        MinX = info.startPointX;
        MaxX = info.endPointX;
        MaxY = info.startPointY > info.endPointY ? info.startPointY : info.endPointY;

        MinY = MaxY - 2 * (Mathf.Abs(info.startPointY - info.endPointY));
        numPontosCorte = ptsCorte;
    }

    public override void Initialize()
    {
        RandomInitialization();
    }

    public override void Mutate(float probability)
    {
        NewValueMutation(probability);
    }

    public override void Crossover(Individual partner, float probability)
    {
        CrossoverNPoints(partner, probability, numPontosCorte);
    }

    // dada um dado genotipo passar para o fenotipo
    public override void CalcTrackPoints()
    {
        //the representation used in the example individual is a list of trackpoints, no need to convert
    }

    // nao é preciso mexer
    // fitness = tempo que demora um objeto sem atrito a descer aquela linha
    public override void CalcFitness()
    {
        fitness = eval.time; //in this case we only consider time
    }


    public override Individual Clone()
    {
        NovoIndividuo newobj = (NovoIndividuo)this.MemberwiseClone();
        newobj.fitness = 0f;
        newobj.trackPoints = new Dictionary<float, float>(this.trackPoints);
        return newobj;
    }



    void RandomInitialization()
    {
        float step = (info.endPointX - info.startPointX) / (info.numTrackPoints - 1);
        float y = 0;

        trackPoints.Add(info.startPointX, info.startPointY);//startpoint
        for (int i = 1; i < info.numTrackPoints - 1; i++)
        {
            y = UnityEngine.Random.Range(MinY, MaxY);
            trackPoints.Add(info.startPointX + i * step, y);
        }
        trackPoints.Add(info.endPointX, info.endPointY); //endpoint
    }

    // mutação que está no enunciado.
    // para qualqer elemento gera um novo valor aleatório
    void NewValueMutation(float probability)
    {
        List<float> keys = new List<float>(trackPoints.Keys);
        foreach (float x in keys)
        {
            //make sure that the startpoint and the endpoint are not mutated 
            if (Math.Abs(x - info.startPointX) < 0.01 || Math.Abs(x - info.endPointX) < 0.01)
            {
                continue;
            }
            if (UnityEngine.Random.Range(0f, 1f) < probability)
            {
                trackPoints[x] = UnityEngine.Random.Range(MinY, MaxY);
            }
        }
    }

    // implemntar um crossover de n pontos de corte
    // o que está agora é apenas com um ponto de corte
    // meter uma flag no unity para alterar coisas
    // num de pontos de cortes -> parametro a configurar
 

    void CrossoverNPoints(Individual partner, float probability, int n)
    {
        List<float> keys = new List<float>(trackPoints.Keys);

        if (UnityEngine.Random.Range(0f, 1f) > probability)
        {
            return;
        }

        // Verificação divisão por 0 e n > numTrackPoints
        if(n == 0 || n > info.numTrackPoints)
        {
            return;
        }

        // se o numPontosCorte == 2
        if(n == 2)
        {
            int crossoverPoint1 = Mathf.FloorToInt(info.numTrackPoints / 3);
            int crossoverPoint2 = crossoverPoint1 * 2;

            for (int i = crossoverPoint1; i < crossoverPoint2; i++)
            {
                float tmp = trackPoints[keys[i]];
                trackPoints[keys[i]] = partner.trackPoints[keys[i]];
                partner.trackPoints[keys[i]] = tmp;
            }

        }
        // Caso seja mais
        else
        {
            int crossoverPoint = Mathf.FloorToInt(info.numTrackPoints / n);

            for (int i = 0; i < crossoverPoint; i++)
            {
                float tmp = trackPoints[keys[i]];
                trackPoints[keys[i]] = partner.trackPoints[keys[i]];
                partner.trackPoints[keys[i]] = tmp;
            }
        }
    }

}

