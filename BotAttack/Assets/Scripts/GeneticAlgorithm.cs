using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{

    //Number of chromosomes
    [SerializeField] private int population = 10;
    //Number of genes in each chromosome
    [SerializeField] private int numberOfGenes = 10;

    //Multipliers for fitness fields
    [SerializeField] private float damageMultiplier = 3.0f;
    [SerializeField] private float timeMultiplier = 1.0f;
    [SerializeField] private float escapeMultiplier = 1.0f;

    //BotData Lists
    public List<BotData> botListSP1 = new List<BotData>();
    public List<BotData> botListSP2 = new List<BotData>();
    public List<BotData> botListSP3 = new List<BotData>();
    public List<BotData> botListSP4 = new List<BotData>();

    //Elite Bots
    //  - Holds an elite bot for each wave
    //  - Each wave's bot is held at the respective element spot (sp1wave is held in element 0, etc.)
    //  - Must be cleared after use
    public List<BotData> eliteBots = new List<BotData>();

    /*
     * Calculates the fitness value for the supplied bot
     *      Fitness Value is determined by:
     *          - Multiplying damage dealt by the damage multiplyer
     *          - Multiply time alive by the time multiplyer
     *          - add those numbers together
     *          - if the bot escaped, multiply the result by the escape multiplyer
     */
    public float FitnessFunction(Bot bot)
    {
        float fitnessValue = 0;

        fitnessValue = (bot.getDamageDealt() * damageMultiplier) + (bot.getTimeAlive() * timeMultiplier);

        if (bot.isEscaped())
        {
            fitnessValue *= escapeMultiplier;
        }

        return fitnessValue;

    }

    /*
     * SelectionFunction - Selects 2 bots using the roulette wheel mechanic
     *      - Sums the fitness values of all the bots in the provided list
     *      - Calculates the normalized fitness values for each bot
     *      - Sort the bot list by their normalized values (Descending order)
     *      - Calculate the cumulative sum for each bot
     *      - Generate a random number from 0 to 1 to use for the roulette wheel
     *      - Pick 2 bots from oposite sides of the roulette wheel
     *      - Add those bots to the parents list and return the parents list
     */
    public List<BotData> SelectionFunction(List<BotData> botList)
    {
        BotData parent1 = new BotData();
        BotData parent2 = new BotData();
        List<BotData> parents = new List<BotData>();
        float fitnessSum = 0;
        List<BotData> sortedList;

        //Sum the fitness values in the botList
        foreach(BotData bot in botList)
        {
            fitnessSum += bot.getFitnessValue();
        }
        //float normalizedTotal = 0;
        //Normalize fitness values
        foreach (BotData bot in botList)
        {
            bot.calcNormalizedFitnessValue(fitnessSum);
            //normalizedTotal += bot.getNormalizedFitnessValue();
        }

        //Debug.Log("Normalized Total: " + normalizedTotal);

        //Sort by normalized values
        sortedList = botList.OrderByDescending(o => o.getNormalizedFitnessValue()).ToList();

        //Calculate the cumulative sum
        for(int i = 0; i < sortedList.Count; ++i)
        {
            for(int j = i; j < sortedList.Count; ++j)
            {
                sortedList.ElementAt(i).calcCumulativeValue(sortedList.ElementAt(j).getNormalizedFitnessValue());
            }
        }

        //Add the most fit bot to the eliteBots list
        eliteBots.Add(sortedList.ElementAt(0));

        //Generate random number between 0 and 1
        float selector = Random.Range(0.0f, 1.0f);

        //Find the bot in the sorted list that has a fitness value that corresponds with the random number
        for(int i = 0; i < sortedList.Count; ++i)
        {
            float lowerBound;
            if(i >= sortedList.Count-1)
            {
                lowerBound = 0;
            }
            else
            {
                lowerBound = sortedList.ElementAt(i + 1).getCumulativeValue();
            }

            //Find Parent 1
            if(selector < sortedList.ElementAt(i).getCumulativeValue() && 
                selector > lowerBound)
            {
                parent1 = sortedList.ElementAt(i);
            }
            //Find parent 2
            if (((selector + .05f) % 1) < sortedList.ElementAt(i).getCumulativeValue() && 
                ((selector + .05f) % 1) > lowerBound) 
            {
                parent2 = sortedList.ElementAt(i);
            }
        }

        parents.Add(parent1);
        parents.Add(parent2);

        return parents;
    }

    /*
     * RecombinationFunction - Combines the genomes of 2 parents that are provided in 
     *                          a list and makes 2 children who have opposing genes
     *      - A crossover point for the genes is chosen at random
     *      - Copies the genes of the parents to the children, switching parents at the crossover point
     *      - Returns a list of 2 children
     */
    public List<BotData> RecombinationFunction(List<BotData> parents)
    {

        List<BotData> children = new List<BotData>();

        BotData child1 = new BotData();
        BotData child2 = new BotData();

        int upperBound = Bot.GENOME_SIZE-1;
        int lowerBound = 1;

        int crossoverPoint = Random.Range(lowerBound, upperBound);

        for(int i = 0; i < Bot.GENOME_SIZE; ++i)
        {
            if(i < crossoverPoint)
            {
                //assign genome of the first parent to child1 before the crossover point
                child1.setGenomeElement(i, parents.ElementAt(0).getGenomeElement(i));
                //assign genome of the second parent to child2 before the crossover point
                child2.setGenomeElement(i, parents.ElementAt(1).getGenomeElement(i));
            }
            else
            {
                //assign genome of the first parent to child1 before the crossover point
                child1.setGenomeElement(i, parents.ElementAt(1).getGenomeElement(i));
                //assign genome of the second parent to child2 before the crossover point
                child2.setGenomeElement(i, parents.ElementAt(0).getGenomeElement(i));
            }
        }

        children.Add(child1);
        children.Add(child2);

        return children;

    }

    /*
     * MutationFunction - Mutates the bots in the provided list
     *      - Chance of mutation per gene is equal to mutationProbability
     *      - Iterates over each gene for each bot and mutates the gene if
     *          the random number between 0 and 1 is less than mutationProbability
     *      - After iterating through the genes of each bot, adds that bot to the mutated bots list
     *      - Returns the mutated bots list
     */
    public List<BotData> MutationFunction(List<BotData> botList)
    {
        float mutationProbability = 0.4f;
        List<BotData> mutatedBots = new List<BotData>();

        foreach(BotData bot in botList)
        {

            for(int i = 0; i < Bot.GENOME_SIZE; ++i)
            {
                float r = Random.Range(0, 1);

                if (r < mutationProbability)
                {
                    bot.setGenomeElement(i, Random.Range(1, 48));
                }
            }

            mutatedBots.Add(bot);

        }

        return mutatedBots;

    }

}

/*
 * BotData class - holds bot data to use in the GA
 * 
 */
public class BotData
{

    private float fitnessValue;
    private float normalizedFitnessValue;
    private int[] genome = new int[Bot.GENOME_SIZE];
    private Vector3 startPoint;
    private int waveNumber;
    private float cumulativeFitnessValue;

    public BotData()
    {
        //for(int i = 0; i < genome.Length)
    }

    public BotData(float fitnessValue, int[] genome)
    {
        this.fitnessValue = fitnessValue;
        this.genome = genome;
    }

    public void setGenomeElement(int element, int value)
    {
        genome[element] = value;
    }

    public int getGenomeElement(int element)
    {
        return genome[element];
    }

    public float getFitnessValue()
    {
        return fitnessValue;
    }

    public void setFitnessValue(float value)
    {
        fitnessValue = value;
    }

    public int[] getGenome()
    {
        return genome;
    }

    public float getNormalizedFitnessValue()
    {
        return normalizedFitnessValue;
    }

    public void calcNormalizedFitnessValue(float sum)
    {
        normalizedFitnessValue = fitnessValue / sum;
    }

    public void calcCumulativeValue(float value)
    {
        cumulativeFitnessValue += value;
    }

    public float getCumulativeValue()
    {
        return cumulativeFitnessValue;
    }

}


/*
class Comparator : IComparer<int>
{
    public int Compare(int x, int y)
    {
        if(x == 0.0f || y == 0.0f)
        {
            return 0;
        }

        return x.CompareTo(y);
    }
}
*/