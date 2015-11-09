using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 7: Non-overlapping Template Matching Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
    /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
    /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
    /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
    /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.
    /// </remarks>
    public class NonOverlappingTemplateMatching : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;
        private const String TEMPLATE_DIR = "RandomNumbers.Resources";
        private const String TEMPLATESPLITSTR = " ";
        private static int M;
        private static int N = 8;
        private static double mu;
        private static double sigma;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The length in bits of each template.
        /// </summary>
        /// <remarks>
        ///  The template is the target string.
        /// </remarks>
        private int m { get; set; }
        /// <summary>
        /// The m-bit template to be matched.
        /// </summary>
        /// <remarks>
        /// The m-bit template to be matched; B is a string of ones and zeros (of length m) which is
        /// defined in a template library of non-periodic patterns contained within the test code.
        /// </remarks>
        private int[] B { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="m">The length in bits of each template</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public NonOverlappingTemplateMatching(int m, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Non Overlapping Template Matching n");
                }
                if (m > 21 || m < 0) {
                    throw new ArgumentException("The value of m must and be between 0 and 21 inclusive", "Non Overlapping Template Matching m");
                }
                this.n = n;
                this.m = m;
                M = n / N;
        }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="B">The m-bit template to match</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref=""
        /// <exception cref="OverflowException"/>
        public NonOverlappingTemplateMatching(String B, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Non Overlapping Template Matching n");
                }
                this.n = n;
                this.B = new int[B.Length];
                for (int i = 0; i < B.Length; i++) {
                    try {
                        this.B[i] = Convert.ToInt32(B.Substring(i, 1));
                    } catch (FormatException) {
                        throw new FormatException("The input data did not consist of a an optional " +
                                "sign followed by a sequence of digits (0 through 9):\r\n\r\n" + B);
                    } catch (OverflowException) {
                        throw new OverflowException("The input string was not of a number within the program's ranges:\r\n\r\n" + B);
                    }
                }

                M = n / N;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public override double[] run(bool printResults) {
            mu = (M - B.Length + 1) / Math.Pow(2, B.Length);
            sigma = M * (1.0 / Math.Pow(2.0, B.Length) - (2.0 * B.Length - 1.0) / Math.Pow(2.0, 2.0 * B.Length));
            return run(B, printResults);
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "7: Non Overrlapping Template Matching Test to run on " + n + " bits compared with a " + m + " length templates";
        }

        private double[] run(int[] B, bool printResults) {
            if (n > model.epsilon.Count || n <= 0) {
                throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
            }

            int[] Wj = new int[N];

            for (int i = 0; i < N; i++) { //for each start position
                for (int j = 0; j < M - B.Length + 1; j++) {    //is there a match for the template
                    bool match = true;
                    for (int k = 0; k < B.Length; k++) {
                        if ((int)B[k] != (int)model.epsilon[i * M + j + k]) {
                            match = false;  
                            break;
                        }
                    }
                    if (match) {
                        Wj[i]++;    //if there was a match, record it
                    }
                }
            }

            //calculate p_value
            double chi2 = 0.0;
            for (int i = 0; i < N; i++) {
                chi2 += Math.Pow(((double)Wj[i] - mu) / Math.Pow(sigma, 0.5), 2);
            }
            double p_value = Cephes.igamc(N / 2.0, chi2 / 2.0);


            return new double[] { p_value };
        }
    }
}
