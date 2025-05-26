using UnityEngine;

public class GlossySpriteEffect : MonoBehaviour {
	[Header("Highlight Settings")]
	public Material glossMaterial;

	public float highlightIntensity = 1.0f;
	public float highlightSpeed = 2.0f;
	public Vector2 highlightDirection = new(1, 1);
	public Color highlightColor = Color.white;

	[Header("Animation")]
	public float animationDuration = 2.0f;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private bool _isPlayAnim;
	private float timer;

	// Property IDs для оптимизации
	private static readonly int HighlightIntensityID = Shader.PropertyToID("_HighlightIntensity");
	private static readonly int HighlightPositionID = Shader.PropertyToID("_HighlightPosition");
	private static readonly int HighlightColorID = Shader.PropertyToID("_HighlightColor");
	private static readonly int HighlightDirectionID = Shader.PropertyToID("_HighlightDirection");

	public void Update () {
		if (_isPlayAnim) AnimateHighlight();
	}

	public void PlayAnim () {
		InitializeMaterial();
		
		_isPlayAnim = true;
	}
	
	private void InitializeMaterial () {
		spriteRenderer.material = new Material(glossMaterial);
		
		spriteRenderer.material.SetFloat(HighlightIntensityID, highlightIntensity);
		spriteRenderer.material.SetColor(HighlightColorID, highlightColor);
		spriteRenderer.material.SetVector(HighlightDirectionID, highlightDirection.normalized);
	}

	private void AnimateHighlight () {
		timer += Time.deltaTime * highlightSpeed;
		
		var normalizedTime = timer % animationDuration / animationDuration;
		var highlightPos = Mathf.PingPong(normalizedTime * 2f, 1f);
		
		spriteRenderer.material.SetFloat(HighlightPositionID, highlightPos);
	}
}
