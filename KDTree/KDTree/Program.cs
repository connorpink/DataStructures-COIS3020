using Assignment3KDTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment3KDTree
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
    public class KDNode
    {
        public Point point;                // node in a kd tree
        public int cutDim;                 // splitting point
        public KDNode left;                // cutting dimension
        public KDNode right;               // children

        public KDNode( Point x, int cutDim) // constructor
        {
            this.point = x;
            this.cutDim = cutDim;
            left = right = null;
        }

        public Boolean inLeftSubtree(Point x) // is x in left subtree?
        {
            return x.Get(cutDim) < point.Get(cutDim);
        }
        public String toString()
        {
            /* return point info */
            return this.point.toString();
        }
    }
    public class KDTree
    {
        private KDNode Root; // set the root node of the tree

        public void insert(Point x)
        {
            Root = insert(x, Root, 0);
        }
        private KDNode insert(Point x, KDNode p, int cutDim)
        {
            if (p == null)                              // fell out of tree
            {
                p = new KDNode(x, cutDim);               // create new leaf
            }
            else if (p.point.Equals(x))
            {
                return p; // point already exists
            }
            else if (p.inLeftSubtree(x))
            {              // insert into left subtree    
                p.left = insert(x, p.left, (p.cutDim + 1) % x.GetDim());
            }
            else                                        //insert into right subtree
            {
                p.right = insert(x, p.right, (p.cutDim + 1) % x.GetDim());
            }
            return p;
        }

        public Point findMin(KDNode p, int i)       //get min point along dim 1
        {
            if (p == null)                          // fell out of tree?
            {
                return null;
            }

            if (p.cutDim == i)                      // cutting dimension matches i?
            {
                if (p.left == null)                 // no left child?
                {
                    return p.point;                 // use this point
                }
                else return findMin(p.left, i);     // get min from left subtree
            }
            else                                    // it may be in the other side
            {
                Point q = minAlongDim(p.point, findMin(p.left, i), i);
                return minAlongDim(q, findMin(p.right, i), i);
            }
        }

        public Point minAlongDim(Point p1, Point p2, int i)     // return smaller point on dim i
        {
            if (p2 == null || p1.Get(i) <= p2.Get(i))                   // p1[i] is short for p1.get(i)
                return p1;
            else
                return p2;
        }

        public void delete(Point x)
        {
            Root = delete(x, Root);
        }
        private KDNode delete(Point x, KDNode p)
        {
            if (p == null)                                  // fell out of tree?
            {
                throw new Exception("point does not exist");
            }
            else if (p.point.Equals(x))                     // found it
            {
                if (p.right != null)                        // take replacement from right
                {
                    p.point = findMin(p.right, p.cutDim);
                    p.right = delete(p.point, p.right);
                }
                else if (p.left != null)                    // take replacement from left
                {
                    p.point = findMin(p.left, p.cutDim);
                    p.right = delete(p.point, p.left);      // move left subtree to right!
                    p.left = null;                          // left subtree is now empty
                }
                else                                        // deleted point in point
                {
                    p = null;                               // remove this leaf
                }
            }
            else if (p.inLeftSubtree(x))
            {
                p.left = delete(x, p.left);                 // delete from left subtree
            }
            else                                            // delte from right subtreex
            {
                p.right = delete(x, p.right);
            }
            return p;
        }
        public void print()
        {
            print(Root, 0);

        }
        public void print(KDNode root, int index)
        {
            if (root != null)
            { 
                print(root.right, index + 5);
                Console.WriteLine(new String(' ', index) + root.toString());
                print(root.left, index + 5);
            }
        }

        public bool contains(Point p)
        {
            return contains(p, Root);
        }
        // Returns true if point p is found; false otherwise
        private bool contains(Point p, KDNode root)
        {
            if (root == null)                                  // fell out of tree?
            {
                return false; // point was not in tree
            }
            else if (root.point.Equals(p))                     // found it
            {
                return true;
            }
            else if (root.inLeftSubtree(p))         // if in left subtree
            {
                return contains(p, root.left);                 // search left subtree
            }
            else                                            
            {
                return contains(p, root.right);                // search right subtree
            }
        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            /* testing for 2 dimensions */
            KDTree A = new KDTree();

            /* testing inserting a point */
            Console.WriteLine("testing inserting testPoint1 {3,4} ");
            Point testPoint1 = new Point(2); // 2 dimensional point
            testPoint1.Set(0, 3.0f); // set point to have x: 3 and y: 4
            testPoint1.Set(1, 4.0f); // set point to have x: 3 and y: 4
            A.insert(testPoint1); // insert point
            A.print();

            /* testing that adding a duplicate point does nothing */
            Console.WriteLine("testing inserting testPoint1 {3,4} again which is a duplicate and should not insert");
            A.insert(testPoint1);
            A.print();

            /* testing inserting 2 points */
            Point testPoint2 = new Point(2); // 2 dimensional point
            testPoint2.Set(0, 1.0f); // set point to have x: 1 and y: 5
            testPoint2.Set(1, 5.0f); // set point to have x: 1 and y: 5
            A.insert(testPoint2); // insert point
            Console.WriteLine("two 2-dimensional points: {3,4} and {1,5}");
            A.print(); // show tree with 2 2 dimensional points

            /* testing inserting many points */
            Console.WriteLine(" Now adding 10 random points:");
            Random rnd = new Random();
            for (int i=0; i < 10; i++)
            {
                Point testPoint = new Point(2);
                testPoint.Set(0, (float)rnd.Next(10));
                testPoint.Set(1, (float)rnd.Next(10));
                A.insert(testPoint);
            }
            A.print();
            /* testing the contains method for testPoint1 {3,4} */
            Console.WriteLine(" A contains testPoint1 {3, 4}? : " + A.contains(testPoint1));

            /* testing the delete method for testPoint1 {3,4} */
            Console.WriteLine(" Now testPoint1 {3, 4} is removed");
            A.delete(testPoint1);
            A.print();

            /* testing the contains method for testPoint1 {3,4} now that it has been removed */
            Console.WriteLine(" A contains testPoint1 {3, 4}? : " + A.contains(testPoint1));
            Console.ReadKey();

            /* testing  removing another point */
            Console.WriteLine(" Now testPoint2 {1, 5} is removed");
            A.delete(testPoint2);
            A.print();
            Console.WriteLine(" A contains testPoint1 {3, 4}? : " + A.contains(testPoint2));

        }
    }
}
