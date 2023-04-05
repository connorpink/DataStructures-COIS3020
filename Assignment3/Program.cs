using System;

namespace Assignment3
{
    internal class Program
    {
        public class Point
        {
            private float[] coord;

            public Point(int dim)
            {
                /* Construct a zero point */
                coord = new float[dim];
            }

            public int GetDim()
            {
                return coord.Length;
            }

            public float Get(int i)
            {
                return coord[i];
            }

            public void Set(int i, float x)
            {
                coord[i] = x;
            }

            public Boolean Equals(Point other)
            {
                /* Compare with another point */
                if (other == null || other.GetDim() != GetDim())
                {
                    return false;
                }

                for (int i = 0; i < GetDim(); i++)
                {
                    if (other.Get(i) != Get(i))
                    {
                        return false;
                    }
                }

                return true;
            }

            public float distanceTo(Point other)
            {
                /* Computer distance to another point */
                if (other == null || other.GetDim() != GetDim())
                {
                    throw new ArgumentException("One of the points is either null or of different dimension");
                }

                float distance = 0.0f;

                for (int i = 0; i < GetDim(); i++)
                {
                    float pointDiff = other.Get(i) - Get(i);
                    distance += (float)Math.Pow(pointDiff, 2);
                }

                return (float)Math.Sqrt(distance);
            }

            public String toString()
            {
                /* Convert to string */
                string[] coordStrings = new string[GetDim()];

                for (int i = 0; i < GetDim(); i++)
                {
                    coordStrings[i] = Get(i).ToString();
                }

                return "(" + string.Join(",", coordStrings) + ")";
            }
        }

        static void Main(string[] args)
        {
            // Create two points with the same dimension
            Point p1 = new Point(3);
            Point p2 = new Point(3);

            // Create extra point for equality testing
            Point p3 = new Point(3);

            // Create extra point for dimensionality testing
            Point p4 = new Point(4);

            // Set coordinates for p1
            p1.Set(0, 1.0f);
            p1.Set(1, 2.0f);
            p1.Set(2, 3.0f);

            // Set coordinates for p2
            p2.Set(0, 4.0f);
            p2.Set(1, 5.0f);
            p2.Set(2, 6.0f);

            // Set coordinates for p3
            p3.Set(0, 1.0f);
            p3.Set(1, 2.0f);
            p3.Set(2, 3.0f);

            // Set coordinates for p4
            p4.Set(0, 7.0f);
            p4.Set(1, 8.0f);
            p4.Set(2, 9.0f);
            p4.Set(3, 10.0f);


            // Test GetDim Method
            Console.WriteLine("Dimension of p1: " + p1.GetDim());
            Console.WriteLine("Dimension of p2: " + p2.GetDim());
            Console.WriteLine("Dimension of p4: " + p4.GetDim());

            // Test get method
            Console.WriteLine("p1 coordinates: (" + p1.Get(0) + ", " + p1.Get(1) + ", " + p1.Get(2) + ")");
            Console.WriteLine("p2 coordinates: (" + p2.Get(0) + ", " + p2.Get(1) + ", " + p2.Get(2) + ")");
            Console.WriteLine("p4 coordinates: (" + p4.Get(0) + ", " + p4.Get(1) + ", " + p4.Get(2) + ", " + p4.Get(3) + ")");

            // Test Equals Method
            Console.WriteLine("p1 equals p2? " + p1.Equals(p2));
            Console.WriteLine("p1 equals p3? " + p1.Equals(p3));

            // Test distanceTo method
            Console.WriteLine("Distance between p1 and p2: " + p1.distanceTo(p2));

            // test ToString method
            Console.WriteLine("String representation of p1: " + p1.toString());
            Console.WriteLine("String representation of p2: " + p2.toString());

            // wait for user input
            Console.ReadLine();
        }
    }
}