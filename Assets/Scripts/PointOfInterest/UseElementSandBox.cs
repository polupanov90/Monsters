public class UseElementSandBox: AbstractUseElement {
    public override void Use(Child child) {
        child.isSit = true;
    }
    public override void Exit(Child child) {
        child.isSit = false;
    }
}