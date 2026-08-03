using System;
using System.Collections.Generic;

/*
    Third-party library source code provided by Hans Telford
    Attribution (CC BY) Hans Telford 2019, 2020, 2021, 2022, 2023, 2024, 2025
    This license lets others distribute, remix, tweak, and build upon the work, 
    even commercially, as long as credit is given for the original creation.
    Free for educational use
*/

namespace BinTree
{
    class BinaryTree<T> where T : IComparable<T>
    {
        // properties
        private Node<T> root;
        private int count;
        private String traversalString;

        // constructor - initialise the Binary Tree structure 
        //               and set up the root node with a null memory address
        public BinaryTree()
        {
            root = null;
            Count = 0;
            traversalString = "";
        }

        // Get the root node
        public Node<T> GetRoot()
        {
            return root;
        }


        // Get and Set for the node count of the Binary Tree structure
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        // Get and Set for the traversal string of the Binary Tree structure
        public String TraversalString
        {
            get { return traversalString; }
            set { traversalString = value; }
        }


        // Get the height of the Binary Tree structure
        // e.g. number of nodes along the longest path from root node to furthest leaf node
        public int GetHeight(Node<T> root)
        {
            if (root == null)
            {
                return 0;
            }
            else
            {
                // compute height of each sub-tree
                int leftSideHeight = GetHeight(root.leftChild);
                int rightSideHeight = GetHeight(root.rightChild);
                // use the larger
                if (leftSideHeight > rightSideHeight)
                {
                    return (leftSideHeight + 1);
                }
                else
                {
                    return (rightSideHeight + 1);
                }
            }
        }

        // Add a node to the Binary Tree structure
        public void Add(T data)
        {
            // Create the node
            Node<T> newNode = new Node<T>(data);
            // check if the root is null
            // if so, assign the root to newNode
            if (root == null)
            {
                root = newNode;
                count++;
                //Console.WriteLine(data + " entered - this is the root");
            }
            else
            {
                Node<T> current = root;
                Node<T> parent;

                while (true)
                {
                    parent = current;
                    // check if data is the same as the parent data
                    // and if so, ignore
                    if (data.CompareTo(current.data) == 0)
                    {
                        // duplicate - ignore the node
                        //Console.WriteLine(data + " entered - duplicate value ignored");
                        return;
                    }
                    // check if data is less than the parent data
                    // and if so, assign current to the left node
                    if (data.CompareTo(current.data) == -1)
                    {
                        current = current.leftChild;
                        if (current == null)
                        {
                            parent.leftChild = newNode;
                            count++;
                            //Console.WriteLine(data + " entered");
                            return;
                        }
                    }
                    // data is now greater than the parent data
                    // in this case, assign current to the right node
                    else
                    {
                        current = current.rightChild;
                        if (current == null)
                        {
                            parent.rightChild = newNode;
                            count++;
                            //Console.WriteLine(data + " entered");
                            return;
                        }
                    }

                } // end while loop

            } // end if-else

        } // end Add() method

        // Contains() method --- looks for a specific value and returns boolean
        // true if found and false if not found
        public bool Contains(T value)
        {
            return (this.Find(value) != null);
        }

        // Find() method called from Contains() method
        public Node<T> Find(T value)
        {
            Node<T> nodeToFind = GetRoot();

            while (nodeToFind != null)
            {
                if (value.CompareTo(nodeToFind.data) == 0)
                {
                    // found
                    return nodeToFind;
                }
                else
                {
                    // search left if the value to find is smaller than the current node
                    if (value.CompareTo(nodeToFind.data) == -1)
                    {
                        nodeToFind = nodeToFind.leftChild;

                    }
                    // search right if the value to find is greater than the current node
                    else if (value.CompareTo(nodeToFind.data) == 1)
                    {
                        nodeToFind = nodeToFind.rightChild;
                    }
                }
            }

            // not found
            return null;
        }

        // Traverse through the Binary Tree structure using 
        // using Preorder method of Root-L-R priority of nodal visitation
        // add to traversalString
        public void Preorder(Node<T> root)
        {
            // Order of method: (Root-L-R)
            if (root != null)
            {
                TraversalString += root.data.ToString() + ", ";
                Preorder(root.leftChild);
                Preorder(root.rightChild);
            }
        }

        // Traverse through the Binary Tree structure 
        // using Postorder method of L-R-Root priority of nodal visitation
        // add to TraversalString
        public void Postorder(Node<T> root)
        {
            // Order of method: (L-R-Root)
            if (root != null)
            {
                Postorder(root.leftChild);
                Postorder(root.rightChild);
                TraversalString += root.data.ToString() + ", ";
            }
        }

        // Traverse through the Binary Tree structure 
        // using Inorder method of L-Root-R priority of nodal visitation
        // This method produces an ordered display of the values
        // add to TraversalString
        public void Inorder(Node<T> root)
        {
            // Order of method: (L-Root-R)

            if (root != null)
            {
                Inorder(root.leftChild);
                TraversalString += root.data.ToString() + ", ";
                Inorder(root.rightChild);

            }
        }

        // Traverse through the Binary Tree structure using Breadth-First method
        // using a queue to systematically traverse every node by level (left -> right and top -> down)
        public void BreadthFirst()
        {
            // breadth-first using a queue
            Queue<Node<T>> q = new Queue<Node<T>>();
            q.Enqueue(this.root);
            while (q.Count > 0)
            {
                Node<T> n = q.Dequeue();
                Console.Write(n.data + " ");
                if (n.leftChild != null)
                {
                    q.Enqueue(n.leftChild);
                }
                if (n.rightChild != null)
                {
                    q.Enqueue(n.rightChild);
                }
            }
        }

        // Traverse through the Binary Tree structure using Depth-First method
        // using a stack to systematically traverse every node starting with the root node
        // and moving down the right side of the root node
        public void DepthFirst()
        {
            // depth-first using a stack
            Stack<Node<T>> s = new Stack<Node<T>>();
            s.Push(this.root);
            while (s.Count > 0)
            {
                Node<T> n = s.Pop();
                Console.Write(n.data + " ");
                if (n.leftChild != null)
                {
                    s.Push(n.leftChild);
                }
                if (n.rightChild != null)
                {
                    s.Push(n.rightChild);
                }
            }
        }
    }
}
