﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GAF;
using System.Drawing;
using AlgoritmosGeneticos.Domain;
using AlgoritmosGeneticos.Helpers;

namespace AlgoritmosGeneticos
{
    public sealed class Logger
    {
        private static Logger instance = null;
        private RichTextBox box;
        private AdapterModelo adapter = AdapterModelo.Instance;
        public Chromosome mejorSolucion;
        public int mejorIteracion = 0;
        public long mejorGeneracion = 0;
        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        public void setTextBox(RichTextBox consola)
        {
            this.box = consola;
        }

        internal void loguearResultados(GaEventArgs e, long cantIteracionesMax)
        {
            Chromosome cromo;
            appendText(Color.Indigo, "Cantidad de individuos: " + e.Population.PopulationSize + ", Cantidad de iteraciones Máxima: " + cantIteracionesMax, true ); 
            appendText(Color.Brown, "El mayor valor de ajuste alcanzado por la población final: " + e.Population.MaximumFitness, true);
            appendText(Color.Red, "Número de iteración: " + e.Generation);
            appendText(Color.Red, ", Cantidad de cromosomas evaluados: " + e.Evaluations, true);

            appendText(Color.BlueViolet, "Mejor solución de la población final:", true);
            cromo = e.Population.Solutions.Find(x => x.Fitness == e.Population.MaximumFitness);
            loguearCromosoma(cromo);

            appendText(Color.BlueViolet, "Mejor solución de la corrida:", true);
            if (mejorIteracion != 0)
            {
                appendText(Color.Red, "Número de iteración: " + mejorIteracion);
                appendText(Color.Red, ", Cantidad de cromosomas evaluados: " + mejorGeneracion, true);
                loguearCromosoma(mejorSolucion);
            }
            else appendText(Color.HotPink, "No hubo una mejor solucion durante la corrida", true);

            appendText(Color.BlueViolet, "Población de la solución:", true);

            foreach (Chromosome cromosoma in e.Population.Solutions)
            {
                loguearCromosoma(cromosoma);
            }

        }

        private void loguearCromosoma(Chromosome cromosoma)
        {
            appendText(Color.CadetBlue, "Aptitud: " + cromosoma.Fitness, true);
            appendText(Color.Blue, cromosoma.ToBinaryString(), true);
            var particiones = Acertijo.particionar(cromosoma.Genes, 9);
            Chromosome cromo = new Chromosome();
            int i = 0;
            foreach (var particion in particiones)
            {
                cromo.Genes.Clear();
                List<Gene> genes = particion.ToList<Gene>();
                cromo.Genes.AddRange(genes);
                appendText(Color.DarkMagenta, cromo.ToBinaryString());
                appendText(Color.Black, "   ---> " + adapter.getModelo(i));
                var genesAuxiliares = Acertijo.particionar(genes, 3);
                int t = 0;
                foreach (var genAuxiliar in genesAuxiliares)
                {
                    cromo.Genes.Clear();
                    cromo.Genes.AddRange(genAuxiliar);
                    string cadenaBits = cromo.ToBinaryString();
                    if (cadenaBits == "111")
                    {
                        appendText(Color.YellowGreen, ", INVALIDO");
                    }
                    else
                    {
                        switch (t)
                        {
                            case 0:
                                appendText(Color.Black, ", " + adapter.getColor(cadenaBits));
                                break;
                            case 1:
                                appendText(Color.Black, ", " + adapter.getPertenencia(cadenaBits));
                                break;
                            case 2:
                                appendText(Color.Black, ", " + adapter.getPosicion(cadenaBits));
                                break;
                            default:
                                appendText(Color.White, ", INVALIDO");
                                break;
                        }
                    }
                    t++;
                }
                i++;
                appendText(Color.Black, " ", true);
            }
        }

        // Append text of the given color.
        public void appendText(Color color, string text, bool AddNewLine = false)
        {
            if (AddNewLine)
            {
                text += Environment.NewLine;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        internal void asignarMejorSolucion(Chromosome cromo, int iteracion, long generacion)
        {
            this.mejorSolucion = cromo;
            this.mejorIteracion = iteracion;
            this.mejorGeneracion = generacion;
        }
    }
}
