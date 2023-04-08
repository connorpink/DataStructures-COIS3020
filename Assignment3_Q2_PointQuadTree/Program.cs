/*==========================================================================|
| PointQuadTree                                                             |
|                                                                           |
| File name: PointQuadTree                                                  |
|                                                                           |
| Written by:   Adrian Lim Zheng Ting                                       |
|               Matthew Makary                                              |
|               Connor Pink                                                 |
|                                                                           |
| Purpose: takes any number of Points of any dimension, and arranges        |
|          them for efficient searches. The points have to be of the        |
|          same dimension.                                                  |
|                                                                           |
|                                                                           |
|==========================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadtree
{

    // Quadtree Node
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
    public class Node
    {
        public Point P { get; set; }        // Color of a node (BLACK, WHITE, or GRAY)
        public Node[] NodeList { get; set; }        // list of all child nodes

        public int dim { get; set;}
        // Constructor
        // Creates a node with color c and four empty children
        // Time complexity: O(1)

        public Node(Point p)
        {
            P = p;
            dim = p.GetDim();
            //math: 2^dim
            // if it's 1d: 1 child
            // 2d: 2^2, 4 children
            // 3d: 2^3, 8 children etc
            NodeList = new Node[(int)Math.Pow(2, p.GetDim())];
        }
    }

    // Region Quadtree

    public class Quadtree
    {
        // Assumptions:
        // 1) The image is a square with dimensions 2^k x 2^k
        // 2) The image is colored either BLACK or WHITE

        private Node root;      // Root of the quadtree
        private int size;      // Length of its side (n = 2^k)

        // Constructor A
        // Creates an empty quadtree
        // Time complexity: O(1)

        public Quadtree()
        {
            root = null;
        }

        // Constructor
        // Creates a quadtree with a root node that already has a value

        public Quadtree(Point p)
        {
            root = new Node(p);
        }

        public bool Insert(Point p)
        {
            //check if the point p has the correct dim
            if (root != null)
            {
                //different dim
                if (p.GetDim() != root.dim)
                {
                    //immediately break out
                    Console.WriteLine("Point does not have the same dim as the root");
                    return false;
                }
            }
            else if (root ==null)
            {
                root = new Node(p);
                return true;
            }
            //========================================================================================================
            //      More fun math time!
            //      If our children are all in one list, how do we determine which child is which?
            //      For example, in a 2D implementation, there will be 4 children. How do we determine
            //          which child is in position 0, and which is in position 1?
            //      
            //      We evaluate it with binary. (Shamelessly stole this idea from UNIX bitmasks)
            //      In a 1D implementation, points are represented as [x].
            //      To determine if a child should be in slot 0 or 1, we evaluate whether the new point,
            //          [a] is smaller or bigger than [x]. Slot 0 means the new node's x is smaller than
            //          the current node's x, 1 means larger.
            //      
            //      If it is smaller, we write down a 0. Otherwise, we write down a 1.
            //
            //      2D example: [x, y].
            //      The children order in the node goes like this:
            //          [[a smaller, b smaller] , [a larger, b smaller], [a smaller, b larger], [a larger, b larger]].
            //      Index:          0                       1                       2                       3
            //
            //      If it is smaller, we write a 0. If it is bigger, we write a 1. We read the input from left to
            //          right, but we write starting from the least significant bit.
            //      
            //      Given [a, b]. If b is bigger but a is smaller, we write "10", which is decimal for 2.
            //      The child is stored in slot 2.
            //
            //      Same for 3D and 4D. e.g. [x bigger, y smaller, z bigger, w bigger] -> 1101 -> 13
            //========================================================================================================
            Node curr;
            curr = root;
           
            while (curr!=null)
            {
                string binary_representation = "";
                
                //run through all dimensions to get a final binary_representation
                for (int i = 0; i < p.GetDim(); i++)
                {
                    if (p.Get(i) < curr.P.Get(i))
                    {
                        binary_representation = "0" + binary_representation;
                    }
                    else
                    {
                        binary_representation = "1" + binary_representation;
                    }
                }

                //now we convert that binary representation to decimal
                int slot = Convert.ToInt32(binary_representation, 2);

                //if it is empty we insert, otherwise we go down
                if (curr.NodeList[slot] == null)
                {
                    curr.NodeList[slot] = new Node(p);
                    return true;
                }
                else
                {
                    curr = curr.NodeList[slot];
                }

            }

            //false if we somehow cannot insert
            return false;
        }


        public bool Contains(Point p)
        {
            //check if the point p has the correct dim
            if (root != null)
            {
                //different dim
                if (p.GetDim() != root.dim)
                {
                    //immediately break out
                    Console.WriteLine("Point does not have the same dim as the root");
                    return false;
                }
            }
            Node curr;
            curr = root;

            //This is exactly the same as the Insert one, except it uses the equal method of the point to compare
            while (curr!=null)
            {
                //if it is equal we return true
                if (p.Equals(curr.P))
                {
                    return true;
                }
                //other wise we go down
                string binary_representation = "";

                //run through all dimensions to get a final binary_representation
                for (int i = 0; i < p.GetDim(); i++)
                {
                    if (p.Get(i) < curr.P.Get(i))
                    {
                        binary_representation = "0" + binary_representation;
                    }
                    else
                    {
                        binary_representation = "1" + binary_representation;
                    }
                }

                //now we convert that binary representation to decimal
                int slot = Convert.ToInt32(binary_representation, 2);
                //and we go down
                curr = curr.NodeList[slot];
                    
            }
            //if we have iterated to the end, it's false
            return false;
            
        }

        public bool Delete(Point p)
        {
            //if the tree contains p
            //We don't check dim as Contains will do it anyway, no point doing it twice
            if (Contains(p))
            {
                Node curr;
                curr = root;

                //This is exactly the same as the Insert one, except it uses the equal method of the point to compare
                while (curr != null)
                {
                    //other wise we go down
                    string binary_representation = "";

                    //run through all dimensions to get a final binary_representation
                    for (int i = 0; i < p.GetDim(); i++)
                    {
                        if (p.Get(i) < curr.P.Get(i))
                        {
                            binary_representation = "0" + binary_representation;
                        }
                        else
                        {
                            binary_representation = "1" + binary_representation;
                        }
                    }

                    //now we convert that binary representation to decimal
                    int slot = Convert.ToInt32(binary_representation, 2);
                    if (curr.NodeList[slot].P.Equals(p))
                    {

                        //to delete a node, we need to also take care of its children
                        //if the next node is the target node to be deleted
                        //we simply reinsert all of its children
                        //we iterate throught the entire tree for n, where n is the number of children
                        //Then we attach the old child to the new spot, including any existing children that 
                        //it already has

                        //make a copy of the nodelist of the node to be deleted
                        Node[] tbd_NodeList = curr.NodeList[slot].NodeList;
                        //delete the node
                        curr.NodeList[slot] = null;

                        //iterate through the deleted child's nodes
                        for (int i = 0; i < tbd_NodeList.Length;i++)
                        {
                            if (tbd_NodeList[i]!=null)
                            {
                                
                                //here we go again
                                //We're re-inserting every child of the node being deleted
                                //So we're re-iterating over the entire tree
                                //Insert takes a point, not a node so I cannot just use that
                                
                                Node internal_curr = root;
                                while (internal_curr !=null)
                                {
                                    string binary_representation2 = "";

                                    //run through all dimensions to get a final binary_representation
                                    for (int j = 0; j < p.GetDim(); j++)
                                    {
                                        if (tbd_NodeList[i].P.Get(j) < internal_curr.P.Get(j))
                                        {
                                            binary_representation2 = "0" + binary_representation2;
                                        }
                                        else
                                        {
                                            binary_representation2 = "1" + binary_representation2;
                                        }
                                    }

                                    //now we convert that binary representation to decimal
                                    int slot2 = Convert.ToInt32(binary_representation2, 2);
                                    if (internal_curr.NodeList[slot2] == null)
                                    {
                                        //we graft the deleted node's child's entire branch to the new spot
                                        internal_curr.NodeList[slot2] = tbd_NodeList[i];
                                        //break out of the loop and move on to the next child
                                        break;
                                    }
                                    else
                                    {
                                        internal_curr = internal_curr.NodeList[slot2];
                                    }
                                }
                            }
                                
                        }
                        return true;
                    }
                    else
                    {
                        curr = curr.NodeList[slot];
                    }
                    

                }
                //if you reached the end of the tree, it means the point does not exist
                return false;
            }
            else
            {
                //return false if the tree does not contain the point
                return false;
            }
        }
        public void Print()
        {
            Print(root, 0);
        }

        private void Print(Node n, int space)
        {

            
            if (n!=null)
            {
                for (int i = 0; i < n.NodeList.Length / 2; i++)
                {
                    Print(n.NodeList[i], space + 4);
                }
                string str = n.P.toString().PadLeft(n.P.toString().Length + space);
                Console.WriteLine(str);
                for (int i = n.NodeList.Length / 2; i < n.NodeList.Length; i++)
                {
                    Print(n.NodeList[i], space + 4);
                }
            }
        }
    }

    class Program
    {
        

        static void Main(string[] args)
        {
            Point A = new Point(2);
            A.Set(0, 12);
            A.Set(1, 6);
            Point B = new Point(2);
            B.Set(0, 1);
            B.Set(1, 11);
            Point C = new Point(2);
            C.Set(0, 3);
            C.Set(1, 7);
            Point D = new Point(2);
            D.Set(0, 8);
            D.Set(1, 2);
            Point E = new Point(2);
            E.Set(0, 19);
            E.Set(1, 1);
            Point F = new Point(2);
            F.Set(0, 4);
            F.Set(1, 4);
            Point G = new Point(2);
            G.Set(0, 2);
            G.Set(1, 3);
            Point H = new Point(2);
            H.Set(0, 5);
            H.Set(1, 9);
            Point J= new Point(2);
            J.Set(0, 3);
            J.Set(1, 13);
            Point K = new Point(2);
            K.Set(0, 14);
            K.Set(1, 22);
            



            Point[] PointList = new Point[10] { A, B, C, D, E, F, G, H, J, K };
            Quadtree Q = new Quadtree();
            
            for (int i = 0; i< PointList.Length; i++ )
            {
                
                Q.Insert(PointList[i]);
            }
            Q.Print();

            //insert wrong dim
            Point L = new Point(3);
            L.Set(0, 0);
            L.Set(1, 0);
            L.Set(2, 0);
            Q.Insert(L);
            Console.WriteLine(Q.Contains(B));
            Console.WriteLine(Q.Contains(C));
            Point M = new Point(2); //non existant point
            M.Set(0, 0);
            M.Set(1, 0);

            Console.WriteLine(Q.Contains(M));
            Q.Delete(B);
            Q.Print();
            for (int i = 0; i < PointList.Length; i++)
            {
                Console.WriteLine(String.Format("Point: {0}, Exists: {1}", (char)(i + 'a'), Q.Contains(PointList[i])));
            }
            Console.WriteLine("*Note that this is slightly wrong as Point I does not exist (as I did not want to confuse 'I' with 'L'. ");
            Console.WriteLine("Point I in the last block is actually Point J in code, and Point J is actually K");

            Console.ReadKey();
            
        }
    }
}
