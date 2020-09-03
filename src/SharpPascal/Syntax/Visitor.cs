namespace SharpPascal.Syntax
{
    public class Visitor
    {
        public virtual bool VisitUnit(Unit unit) => true;

        public virtual void VisitVarDeclaration(VarDeclaration @var) { }

        public virtual bool VisitIfStatement(IfStatement @if) => true;

        public virtual bool VisitWhileStatement(WhileStatement @while) => true;

        public virtual bool VisitAssignmentStatement(AssignmentStatement assign) => true;

        public virtual void VisitIntegerExpression(IntegerExpression integer) { }

        public virtual void VisitStringExpression(StringExpression @string) { }

        public virtual void VisitVarExpression(VarExpression @var) { }

        public virtual void VisitAddExpression(AddExpression add) { }

        public virtual void VisitSubExpression(SubExpression sub) { }

        public virtual void VisitMulExpression(MulExpression mul) { }

        public virtual void VisitDivExpression(DivExpression div) { }

        public virtual void VisitModExpression(ModExpression mod) { }

        public virtual void VisitEqualExpression(EqualExpression equal) { }

        public virtual void VisitNotEqualExpression(NotEqualExpression notEqual) { }

        public virtual void VisitLessThanExpression(LessThanExpression lessThan) { }

        public virtual void VisitGreaterThanExpression(GreaterThanExpression greaterThan) { }

        public virtual void VisitLessOrEqualExpression(LessOrEqualExpression lessOrEqual) { }

        public virtual void VisitGreaterOrEqualExpression(GreaterOrEqualExpression greaterOrEqual) { }

        public virtual bool VisitCallExpression(CallExpression call) => true;
    }
}