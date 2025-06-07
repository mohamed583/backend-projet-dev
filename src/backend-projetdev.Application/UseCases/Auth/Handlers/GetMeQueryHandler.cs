using AutoMapper;
using backend_projetdev.Application.Common;
using backend_projetdev.Application.DTOs;
using backend_projetdev.Application.Interfaces;
using backend_projetdev.Application.UseCases.Auth.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_projetdev.Application.UseCases.Auth.Handlers
{
    public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<PersonneDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICandidatRepository _candidatRepository;
        private readonly IFormateurRepository _formateurRepository;
        private readonly IEmployeService _employeService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public GetMeQueryHandler(
            ICurrentUserService currentUserService,
            ICandidatRepository candidatRepository,
            IFormateurRepository formateurRepository,
            IEmployeService employeService,
            IRoleService roleService,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _candidatRepository = candidatRepository;
            _formateurRepository = formateurRepository;
            _employeService = employeService;
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<Result<PersonneDto>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var userId = await _currentUserService.GetUserIdAsync();

            if (string.IsNullOrEmpty(userId))
                return Result<PersonneDto>.Failure("User is not authenticated");

            var roles = await _roleService.GetUserRolesAsync(userId);

            if (roles.Contains("Candidat"))
            {
                var candidat = await _candidatRepository.GetByIdAsync(userId);
                if (candidat == null) return Result<PersonneDto>.Failure("Candidat not found");

                return Result<PersonneDto>.SuccessResult(_mapper.Map<PersonneDto>(candidat));
            }

            if (roles.Contains("Formateur"))
            {
                var formateur = await _formateurRepository.GetByIdAsync(userId);
                if (formateur == null) return Result<PersonneDto>.Failure("Formateur not found");

                return Result<PersonneDto>.SuccessResult(_mapper.Map<PersonneDto>(formateur));
            }

            var employe = await _employeService.GetByIdAsync(userId);
            if (employe == null) return Result<PersonneDto>.Failure("Employé not found");

            return Result<PersonneDto>.SuccessResult(_mapper.Map<PersonneDto>(employe));
        }
    }
}