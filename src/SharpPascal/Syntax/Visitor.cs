using System;

namespace SharpPascal.Syntax
{
    public class Visitor
    {
        public Func<UnitSyntax, bool> VisitUnit = _ => true;

        public Action<VarDeclarationSyntax> VisitVarDeclaration = _ => { };

        public Func<IfStatementSyntax, bool> VisitIfStatement = _ => true;

        public Func<WhileStatementSyntax, bool> VisitWhileStatement = _ => true;

        public Func<AssignmentStatementSyntax, bool> VisitAssignmentStatement = _ => true;

        public Action<IntegerExpressionSyntax> VisitIntegerExpression = _ => { };

        public Action<StringExpressionSyntax> VisitStringExpression = _ => { };

        public Action<VarExpressionSyntax> VisitVarExpression = _ => { };

        public Action<AddExpressionSyntax> VisitAddExpression = _ => { };

        public Action<SubExpressionSyntax> VisitSubExpression = _ => { };

        public Action<MulExpressionSyntax> VisitMulExpression = _ => { };

        public Action<DivExpressionSyntax> VisitDivExpression = _ => { };

        public Action<ModExpressionSyntax> VisitModExpression = _ => { };

        public Action<EqualExpressionSyntax> VisitEqualExpression = _ => { };

        public Action<NotEqualExpressionSyntax> VisitNotEqualExpression = _ => { };

        public Action<LessThanExpressionSyntax> VisitLessThanExpression = _ => { };

        public Action<GreaterThanExpressionSyntax> VisitGreaterThanExpression = _ => { };

        public Action<LessOrEqualExpressionSyntax> VisitLessOrEqualExpression = _ => { };

        public Action<GreaterOrEqualExpressionSyntax> VisitGreaterOrEqualExpression = _ => { };

        public Func<CallExpressionSyntax, bool> VisitCallExpression = _ => true;
    }
}
