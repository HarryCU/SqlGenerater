/**
 * Copyright (c) 2015, Harry CU 邱允根 (292350862@qq.com).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Linq;
using System.Linq.Expressions;

namespace SqlGenerater.Utils
{
    public static class ExpressionExtendsion
    {
        public static Expression UnpackToParamterExpr(this Expression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.New:
                    var exprNew = (NewExpression)body;
                    return ((MemberExpression)exprNew.Arguments.First()).Expression;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)body).Expression;
            }
            return body;
        }

        public static bool IsOpCondition(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOpComparison(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetOperatorPrecedence(this Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return 10;
                case ExpressionType.And:
                    return 6;
                case ExpressionType.AndAlso:
                    return 3;
                case ExpressionType.Coalesce:
                case ExpressionType.Assign:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                    return 1;
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    return 15;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.Throw:
                case ExpressionType.Unbox:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.OnesComplement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                    return 12;
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return 11;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return 7;
                case ExpressionType.ExclusiveOr:
                    return 5;
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.TypeEqual:
                    return 8;
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    return 9;
                case ExpressionType.Or:
                    return 4;
                case ExpressionType.OrElse:
                    return 2;
                case ExpressionType.Power:
                    return 13;
            }
            return 14;
        }

        public static bool NeedsParentheses(this Expression parent, Expression child)
        {
            if (child == null)
            {
                return false;
            }
            ExpressionType nodeType = parent.NodeType;
            if (nodeType <= ExpressionType.Increment)
            {
                if (nodeType != ExpressionType.Decrement && nodeType != ExpressionType.Increment)
                {
                    return NeedsParenthesesNext(parent, child);
                }
            }
            else if (nodeType != ExpressionType.Unbox && nodeType != ExpressionType.IsTrue && nodeType != ExpressionType.IsFalse)
            {
                return NeedsParenthesesNext(parent, child);
            }
            return true;
        }

        private static bool NeedsParenthesesNext(Expression parent, Expression child)
        {
            int operatorPrecedence = child.GetOperatorPrecedence();
            int operatorPrecedence2 = parent.GetOperatorPrecedence();
            if (operatorPrecedence == operatorPrecedence2)
            {
                var nodeType = parent.NodeType;
                if (nodeType <= ExpressionType.MultiplyChecked)
                {
                    if (nodeType <= ExpressionType.Divide)
                    {
                        switch (nodeType)
                        {
                            case ExpressionType.Add:
                            case ExpressionType.AddChecked:
                                break;
                            case ExpressionType.And:
                            case ExpressionType.AndAlso:
                                return false;
                            default:
                                if (nodeType != ExpressionType.Divide)
                                {
                                    return true;
                                }
                                return NeedsParenthesesLast(parent, child);
                        }
                    }
                    else
                    {
                        if (nodeType == ExpressionType.ExclusiveOr)
                        {
                            return false;
                        }
                        switch (nodeType)
                        {
                            case ExpressionType.Modulo:
                                return NeedsParenthesesLast(parent, child);
                            case ExpressionType.Multiply:
                            case ExpressionType.MultiplyChecked:
                                break;
                            default:
                                return true;
                        }
                    }
                    return false;
                }
                if (nodeType <= ExpressionType.OrElse)
                {
                    if (nodeType != ExpressionType.Or && nodeType != ExpressionType.OrElse)
                    {
                        return true;
                    }
                }
                else
                {
                    if (nodeType != ExpressionType.Subtract && nodeType != ExpressionType.SubtractChecked)
                    {
                        return true;
                    }
                    return NeedsParenthesesLast(parent, child);
                }
                return false;
            }
            return (child.NodeType == ExpressionType.Constant && (parent.NodeType == ExpressionType.Negate || parent.NodeType == ExpressionType.NegateChecked)) || operatorPrecedence < operatorPrecedence2;
        }

        private static bool NeedsParenthesesLast(Expression parent, Expression child)
        {
            BinaryExpression binaryExpression = parent as BinaryExpression;
            return binaryExpression != null && child == binaryExpression.Right;
        }
    }
}
