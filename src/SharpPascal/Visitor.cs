using System;

namespace SharpPascal
{
    public class Visitor
    {
        public Func<Unit, bool> VisitUnit = _ => true;

        public Action<VarDeclaration> VisitVarDeclaration = _ => { };

        public Func<IfStatement, bool> VisitIfStatement = _ => true;

        public Func<WhileStatement, bool> VisitWhileStatement = _ => true;

        public Func<AssignmentStatement, bool> VisitAssignmentStatement = _ => true;

        public Action<IntegerExpression> VisitIntegerExpression = _ => { };

        public Action<StringExpression> VisitStringExpression = _ => { };

        public Action<VarExpression> VisitVarExpression = _ => { };

        public Action<AddExpression> VisitAddExpression = _ => { };

        public Action<SubExpression> VisitSubExpression = _ => { };

        public Action<MulExpression> VisitMulExpression = _ => { };

        public Action<DivExpression> VisitDivExpression = _ => { };

        public Action<ModExpression> VisitModExpression = _ => { };

        public Action<EqualExpression> VisitEqualExpression = _ => { };

        public Action<NotEqualExpression> VisitNotEqualExpression = _ => { };

        public Action<LessThanExpression> VisitLessThanExpression = _ => { };

        public Action<GreaterThanExpression> VisitGreaterThanExpression = _ => { };

        public Action<LessOrEqualExpression> VisitLessOrEqualExpression = _ => { };

        public Action<GreaterOrEqualExpression> VisitGreaterOrEqualExpression = _ => { };

        public Func<CallExpression, bool> VisitCallExpression = _ => true;
    }
}
