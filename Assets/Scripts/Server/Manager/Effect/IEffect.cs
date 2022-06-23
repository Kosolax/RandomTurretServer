public interface IEffect
{
    float Duration { get; set; }

    float Value { get; set; }

    void ApplyEffect();

    void Expire();
}