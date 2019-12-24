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
     *      - Converts a list of BotData in a list sorted by cumulative fitness value
     *      - Set aside an elite bot
     *      - Generate a random number from 0 to 1 to use for the roulette wheel
     *      - Pick 2 bots from oposite sides of the roulette wheel
     *      - Add those bots to the parents list and return the parents list
     */
    public List<BotData> SelectionFunction(List<BotData> botList, int spawnPoint)
    {
        BotData parent1 = new BotData();
        BotData parent2 = new BotData();
        List<BotData> parents = new List<BotData>();
        
        List<BotData> sortedList;
        /*
        foreach(BotData bot in botList)
        {
            Debug.Log("Bot: " + bot.getGenome().ElementAt(0) + ", " 
                + bot.getGenome().ElementAt(1) + ", " 
                + bot.getGenome().ElementAt(2) + ", ");
            Debug.Log("Bot Fitness: " + bot.getFitnessValue());
        }
        */
        //Get a list of BotData sorted by cumulative fitness value
        sortedList = getSortedList(botList);
        /*
        foreach(BotData bot in sortedList)
        {
            Debug.Log("Bot: " + bot.getGenome().ElementAt(0) + ", "
                + bot.getGenome().ElementAt(1) + ", "
                + bot.getGenome().ElementAt(2) + ", ");
            Debug.Log("Bot Cumulative Fitness: " + bot.getCumulativeValue());
        }
        */
        //Add the most fit bot to the eliteBots list
        if (spawnPoint == 1)
        {
            eliteBots.Add(sortedList.ElementAt(0));
            eliteBots.ElementAt(0).setElite(true);
        }
        else if(spawnPoint == 2)
        {
            eliteBots.Add(sortedList.ElementAt(0));
            eliteBots.ElementAt(1).setElite(true);
        }
        else if(spawnPoint == 3)
        {
            eliteBots.Add(sortedList.ElementAt(0));
            eliteBots.ElementAt(2).setElite(true);
        }
        else if (spawnPoint == 4)
        {
            eliteBots.Add(sortedList.ElementAt(0));
            eliteBots.ElementAt(3).setElite(true);
        }
        
        //Generate random number between 0 and 1 for the first parent
        float selector = Random.Range(0.0f, 1.0f);

        //Debug.Log("Random Number: " + selector);

        //Find the bot in the sorted list that has a fitness value that corresponds with the random number
        for (int i = 0; i < sortedList.Count; ++i)
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
                sortedList.RemoveAt(i);
            }
        }
        /*
        Debug.Log("Chosen Bot: " + parent1.getGenome().ElementAt(0) + ", "
                + parent1.getGenome().ElementAt(1) + ", "
                + parent1.getGenome().ElementAt(2) + ", ");
                */

        sortedList = getSortedList(sortedList);

        //Generate random number between 0 and 1 for the second parent
        selector = Random.Range(0.0f, 1.0f);
        //Debug.Log("Selector 2: " + selector);
        //Find the bot in the sorted list that has a fitness value that corresponds with the random number
        for (int i = 0; i < sortedList.Count; ++i)
        {
            //Debug.Log("List C Value: " + sortedList.ElementAt(i).getCumulativeValue());
            float lowerBound;
            if (i >= sortedList.Count - 1)
            {
                lowerBound = 0;
            }
            else
            {
                lowerBound = sortedList.ElementAt(i + 1).getCumulativeValue();
            }

            //Find Parent 2
            if (selector < sortedList.ElementAt(i).getCumulativeValue() &&
                selector > lowerBound)
            {
                parent2 = sortedList.ElementAt(i);
                //Debug.Log("Parent2 C Value: " + parent2.getCumulativeValue());
                //Debug.Log("Parent2 Fitness: " + parent2.getFitnessValue());
                sortedList.RemoveAt(i);
            }
        }

        parents.Add(parent1);
        parents.Add(parent2);

        return parents;
    }

    /*
     *  getSortedList - A utility function to take a list of BotData and return it sorted by cumulative fitness value
     *      - Sums the fitness values of all the bots in the provided list
     *      - Calculates the normalized fitness values for each bot
     *      - Sort the bot list by their normalized values (Descending order)
     *      - Calculate the cumulative sum for each bot
     */
    private List<BotData> getSortedList(List<BotData> botList)
    {
        float fitnessSum = 0;
        List<BotData> sortedList;

        foreach(BotData bot in botList)
        {
            bot.clearNormalValue();
            bot.clearCumulativeValue();
        }

        //Sum the fitness values in the botList
        foreach (BotData bot in botList)
        {
            fitnessSum += bot.getFitnessValue();
        }
        //Debug.Log("Fitness Sum: " + fitnessSum);

        //float normalizedTotal = 0;
        //Normalize fitness values
        foreach (BotData bot in botList)
        {
            bot.calcNormalizedFitnessValue(fitnessSum);
            //normalizedTotal += bot.getNormalizedFitnessValue();
        }

        //Sort by normalized values
        sortedList = botList.OrderByDescending(o => o.getNormalizedFitnessValue()).ToList();
        /*
        foreach (BotData bot in sortedList)
        {
            Debug.Log("FitnessValue: " + bot.getNormalizedFitnessValue());
        }
        */
        //Calculate the cumulative sum
        for (int i = 0; i < sortedList.Count; ++i)
        {
            for (int j = i; j < sortedList.Count; ++j)
            {
                //Debug.Log("NV: " + sortedList.ElementAt(j).getNormalizedFitnessValue());
                sortedList.ElementAt(i).calcCumulativeValue(sortedList.ElementAt(j).getNormalizedFitnessValue());
            }
            //Debug.Log("CV: " + sortedList.ElementAt(i).getCumulativeValue());
            
        }
        //Debug.Log("*************");
        return sortedList;
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
        /*
        int count = 1;
        foreach (BotData parent in parents)
        {
            
            Debug.Log("Parent " + count + ": " + parent.getGenome().ElementAt(0)
                + parent.getGenome().ElementAt(1)
                + parent.getGenome().ElementAt(2)
                + parent.getGenome().ElementAt(3)
                + parent.getGenome().ElementAt(4)
                + parent.getGenome().ElementAt(5)
                + parent.getGenome().ElementAt(6)
                + parent.getGenome().ElementAt(7)
                + parent.getGenome().ElementAt(8)
                + parent.getGenome().ElementAt(9)
                + parent.getGenome().ElementAt(10)
                + parent.getGenome().ElementAt(11)
                + parent.getGenome().ElementAt(12)
                + parent.getGenome().ElementAt(13)
                + parent.getGenome().ElementAt(14)
                + parent.getGenome().ElementAt(15)
                + parent.getGenome().ElementAt(16)
                + parent.getGenome().ElementAt(17)
                + parent.getGenome().ElementAt(18)
                + parent.getGenome().ElementAt(18));
            count++;
        }
        */
        List<BotData> children = new List<BotData>();

        BotData child1 = new BotData();
        BotData child2 = new BotData();

        int upperBound1 = Bot.GENOME_SIZE-2;
        int lowerBound1 = 1;

        int crossoverPoint1 = Random.Range(lowerBound1, upperBound1);
        //Debug.Log("Crossover Point 1: " + crossoverPoint1);

        int upperBound2 = Bot.GENOME_SIZE - 1;
        int lowerBound2 = crossoverPoint1 + 1;

        int crossoverpoint2 = Random.Range(lowerBound2, upperBound2);
        //Debug.Log("Crossover Point 2: " + crossoverpoint2);

        //Recorder recorder = GameObject.Find("CoreGame").GetComponent<Recorder>();
        //recorder.recordParents(parents, crossoverPoint);

        for (int i = 0; i < Bot.GENOME_SIZE; ++i)
        {
            if(i < crossoverPoint1)
            {
                //assign genome of the first parent to child1 before the crossover point
                child1.setGenomeElement(i, parents.ElementAt(0).getGenomeElement(i));
                //assign genome of the second parent to child2 before the crossover point
                child2.setGenomeElement(i, parents.ElementAt(1).getGenomeElement(i));
            }
            else if(i < crossoverpoint2)
            {
                //assign genome of the first parent to child1 before the crossover point
                child1.setGenomeElement(i, parents.ElementAt(1).getGenomeElement(i));
                //assign genome of the second parent to child2 before the crossover point
                child2.setGenomeElement(i, parents.ElementAt(0).getGenomeElement(i));
            }
            else
            {
                //assign genome of the first parent to child1 before the crossover point
                child1.setGenomeElement(i, parents.ElementAt(0).getGenomeElement(i));
                //assign genome of the second parent to child2 before the crossover point
                child2.setGenomeElement(i, parents.ElementAt(1).getGenomeElement(i));
            }
        }

        children.Add(child1);
        children.Add(child2);

        /*
        count = 1;
        foreach (BotData child in children)
        {
            Debug.Log("Child " + count + ": " + child.getGenome().ElementAt(0)
                + child.getGenome().ElementAt(1)
                + child.getGenome().ElementAt(2)
                + child.getGenome().ElementAt(3)
                + child.getGenome().ElementAt(4)
                + child.getGenome().ElementAt(5)
                + child.getGenome().ElementAt(6)
                + child.getGenome().ElementAt(7)
                + child.getGenome().ElementAt(8)
                + child.getGenome().ElementAt(9)
                + child.getGenome().ElementAt(10)
                + child.getGenome().ElementAt(11)
                + child.getGenome().ElementAt(12)
                + child.getGenome().ElementAt(13)
                + child.getGenome().ElementAt(14)
                + child.getGenome().ElementAt(15)
                + child.getGenome().ElementAt(16)
                + child.getGenome().ElementAt(17)
                + child.getGenome().ElementAt(18)
                + child.getGenome().ElementAt(19));
            count++;
        }
        */
        //recorder.recordChildren(children);

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
        /*
        Debug.Log("Original Bot: " + botList.ElementAt(0).getGenome().ElementAt(0) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(1) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(2) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(3) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(4) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(5) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(6) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(7) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(8) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(9) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(10) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(11) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(12) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(13) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(14) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(15) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(16) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(17) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(18) + ", "
            + botList.ElementAt(0).getGenome().ElementAt(19));
            */
        float mutationProbability = 0.1f;
        List<BotData> mutatedBots = new List<BotData>();

        foreach(BotData bot in botList)
        {

            for(int i = 0; i < Bot.GENOME_SIZE; ++i)
            {
                float r = Random.Range(0.0f, 1.0f);

                if (r < mutationProbability)
                {
                    Debug.Log("Mutation at element " + i);
                    bot.setGenomeElement(i, Random.Range(1, 165));
                }
            }
            /*
            Debug.Log("Mutated Bot: " + bot.getGenome().ElementAt(0) + ", "
            + bot.getGenome().ElementAt(1) + ", "
            + bot.getGenome().ElementAt(2) + ", "
            + bot.getGenome().ElementAt(3) + ", "
            + bot.getGenome().ElementAt(4) + ", "
            + bot.getGenome().ElementAt(5) + ", "
            + bot.getGenome().ElementAt(6) + ", "
            + bot.getGenome().ElementAt(7) + ", "
            + bot.getGenome().ElementAt(8) + ", "
            + bot.getGenome().ElementAt(9) + ", "
            + bot.getGenome().ElementAt(10) + ", "
            + bot.getGenome().ElementAt(11) + ", "
            + bot.getGenome().ElementAt(12) + ", "
            + bot.getGenome().ElementAt(13) + ", "
            + bot.getGenome().ElementAt(14) + ", "
            + bot.getGenome().ElementAt(15) + ", "
            + bot.getGenome().ElementAt(16) + ", "
            + bot.getGenome().ElementAt(17) + ", "
            + bot.getGenome().ElementAt(18) + ", "
            + bot.getGenome().ElementAt(19));
            */
            mutatedBots.Add(bot);

        }

        //Recorder recorder = GameObject.Find("CoreGame").GetComponent<Recorder>();
        //recorder.recordMutated(mutatedBots);

        return mutatedBots;

    }

    public float getDamageMultiplier()
    {
        return damageMultiplier;
    }

    public float getTimeMultiplier()
    {
        return timeMultiplier;
    }

    public float getEscapeMultiplier()
    {
        return escapeMultiplier;
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
    private bool isElite = false;

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

    public void setElite(bool elite)
    {
        isElite = elite;
    }

    public bool getElite()
    {
        return isElite;
    }

    public void clearNormalValue()
    {
        normalizedFitnessValue = 0;
    }

    public void clearCumulativeValue()
    {
        cumulativeFitnessValue = 0;
    }

}