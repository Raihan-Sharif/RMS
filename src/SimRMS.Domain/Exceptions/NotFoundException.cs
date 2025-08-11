/// <summary>
/// <para>
/// ===================================================================
/// Title:       NotFoundException
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents a custom exception for not found errors in the domain.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
    }
}
