using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase_Scr : MonoBehaviour
{
    public enum IDkeys
    {
        wall,
        obstacle,
        corpse,
        player,
        enemy
    }
    public static Dictionary<IDkeys, int> IDDict = new Dictionary<IDkeys, int>()
    {
        [IDkeys.wall] = -300,
        [IDkeys.obstacle] = -200,
        [IDkeys.corpse] = -100,
        [IDkeys.player] = 300,
        [IDkeys.enemy] = 400
    };

    public struct Matrix2x2
    {
        public float[,] matrixArray;

        public Matrix2x2(float[,] mtrixArray)
        {
            matrixArray = mtrixArray;
        }

        static public Matrix2x2 Multiply(Matrix2x2 firstMatrix, Matrix2x2 secondMatrix)
        {
            Matrix2x2 result = new Matrix2x2(new float[2, 2]);

            result.matrixArray[0, 0] = firstMatrix.matrixArray[0, 0] * secondMatrix.matrixArray[0, 0] + firstMatrix.matrixArray[0, 1] * secondMatrix.matrixArray[1, 0];
            result.matrixArray[0, 1] = firstMatrix.matrixArray[0, 0] * secondMatrix.matrixArray[0, 1] + firstMatrix.matrixArray[0, 1] * secondMatrix.matrixArray[1, 1];
            result.matrixArray[1, 0] = firstMatrix.matrixArray[1, 0] * secondMatrix.matrixArray[0, 0] + firstMatrix.matrixArray[1, 1] * secondMatrix.matrixArray[1, 0];
            result.matrixArray[1, 1] = firstMatrix.matrixArray[1, 0] * secondMatrix.matrixArray[0, 1] + firstMatrix.matrixArray[1, 1] * secondMatrix.matrixArray[1, 1];

            return result;
        }
        public Vector2 Multiply(Vector2 vector)
        {
            Vector2 result = new Vector2();

            result.x = vector.x * matrixArray[0, 0] + vector.y * matrixArray[1, 0];
            result.y = vector.x * matrixArray[0, 1] + vector.y * matrixArray[1, 1];

            return result;
        }
        public Matrix2x2 RotateLeft()
        {
            Matrix2x2 leftMatrix = new Matrix2x2(new float[,] { { 0, 1 }, { -1, 0 } });
            return Multiply(this, leftMatrix);
        }
        public Matrix2x2 RotateRight()
        {
            Matrix2x2 rightMatrix = new Matrix2x2(new float[,] { { 0, -1 }, { 1, 0 } });
            return Multiply(this, rightMatrix);
        }
    }

    /// <summary>
    /// Метод сферической интерполяции с изменённым центром вращения
    /// </summary>
    /// <param name="startPos">Начальный вектор</param>
    /// <param name="endPos">Конечный вектор</param>
    /// <param name="center">Новый центр сферической интерполяции</param>
    /// <param name="t">Величина интерполяции</param>
    /// <returns>Сферически интерполированный вектор</returns>
    public static Vector3 SlerpWO(Vector3 startPos, Vector3 endPos, Vector3 center, float t)
    {
        return Vector3.Slerp(startPos - center, endPos - center, t) + center;
    }
}
